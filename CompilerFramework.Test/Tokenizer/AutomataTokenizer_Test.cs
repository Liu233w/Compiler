using System.Collections.Generic;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.CompilerFramework.Tokenizer.Exceptions;
using Shouldly;
using Xunit;

namespace Liu233w.Compiler.CompilerFramework.Test.Tokenizer
{
    public class AutomataTokenizer_Test
    {
        private readonly AutomataTokenizerState _nameState;

        private readonly AutomataTokenizerState _nameWithCommentState;

        public AutomataTokenizer_Test()
        {
            var nameEndState = AutomataTokenizerState.ForEnd(char.IsLetterOrDigit, "name");
            nameEndState.NextStates.Add(nameEndState);

            _nameState = AutomataTokenizerState.ForBegin(new List<AutomataTokenizerState>
            {
                AutomataTokenizerState.ForEnd(char.IsLetter, "name", new List<AutomataTokenizerState>{nameEndState}),
            });

            var commentBody = AutomataTokenizerState.ForMiddle(c => true, new List<AutomataTokenizerState>
            {
                AutomataTokenizerState.ForMiddle('*'.MatchCurrentPosition(), new List<AutomataTokenizerState>
                {
                    AutomataTokenizerState.ForEnd('/'.MatchCurrentPosition(), "comment"),
                }),
                // commentBody
            });
            commentBody.NextStates.Add(commentBody);

            _nameWithCommentState = AutomataTokenizerState.ForBegin(new List<AutomataTokenizerState>
            {
                AutomataTokenizerState.ForMiddle(char.IsLetter, new List<AutomataTokenizerState> {nameEndState}),
                AutomataTokenizerState.ForMiddle('/'.MatchCurrentPosition(), new List<AutomataTokenizerState>
                {
                    AutomataTokenizerState.ForMiddle('*'.MatchCurrentPosition(),
                        new List<AutomataTokenizerState> {commentBody}),
                })
            });
        }

        [Fact]
        public void 应该能够识别名字()
        {
            var res = AutomataTokenizer.GetByAutomata(_nameState, "name1", 0, out var end);

            res.ShouldMatchObject(new Token("name1", "name", 0, 5));
            end.ShouldBe(5);
        }

        [Fact]
        public void 不能识别的时候应该抛出异常()
        {
            var exception =
                Should.Throw<WrongTokenException>(() =>
                    AutomataTokenizer.GetByAutomata(_nameState, "123", 0, out _));

            exception.Buffer.ShouldBe("123");
            exception.TokenBegin.ShouldBe(0);
            exception.CurrentIdx.ShouldBe(0);
            exception.CurrentState.ShouldBe(_nameState);
        }

        [Fact]
        public void 可以识别目前识别的子串()
        {
            var res = AutomataTokenizer.GetByAutomata(_nameState, "name1.1", 0, out var end);

            res.ShouldMatchObject(new Token("name1", "name", 0, 5));
            end.ShouldBe(5);
        }

        [Fact]
        public void 能够识别单个字符作为的名字()
        {
            var res = AutomataTokenizer.GetByAutomata(_nameState, "a", 0, out var end);

            res.ShouldMatchObject(new Token("a", "name", 0, 1));
            end.ShouldBe(1);
        }

        [Fact]
        public void 在包含多个终止状态时能够正确识别_name在前()
        {
            const string buffer = "aaa/*asddg*/";

            var res = AutomataTokenizer.GetByAutomata(_nameWithCommentState, buffer, 0, out var end);
            res.ShouldMatchObject(new Token("aaa", "name", 0, 3));
            end.ShouldBe(3);

            res = AutomataTokenizer.GetByAutomata(_nameWithCommentState, buffer, end, out end);
            res.ShouldMatchObject(new Token("/*asddg*/", "comment", 3, 12));
            end.ShouldBe(12);
        }

        [Fact]
        public void 在包含多个终止状态时能够正确识别_comment在前()
        {
            const string buffer = "/*asddg*/aaa";

            var res = AutomataTokenizer.GetByAutomata(_nameWithCommentState, buffer, 0, out var end);
            res.ShouldMatchObject(new Token("/*asddg*/", "comment", 0, 9));
            end.ShouldBe(9);

            res = AutomataTokenizer.GetByAutomata(_nameWithCommentState, buffer, end, out end);
            res.ShouldMatchObject(new Token("aaa", "name", 9, 12));
            end.ShouldBe(12);
        }
    }
}
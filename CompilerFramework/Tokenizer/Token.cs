namespace Liu233w.Compiler.CompilerFramework.Tokenizer
{
    /// <summary>
    /// 表示一个语法单元的数据结构
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 语法单元的
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 语法单元的类型
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        /// 语法单元的第一个字符在源代码中的位置
        /// </summary>
        public int TokenBeginIdx { get; set; }

        /// <summary>
        /// 语法单元的最后一个字符的下个字符在源代码中的位置。
        /// </summary>
        public int TokenEndIdx { get; set; }

        public Token(string content, string tokenType, int tokenBeginIdx, int tokenEndIdx)
        {
            Content = content;
            TokenType = tokenType;
            TokenBeginIdx = tokenBeginIdx;
            TokenEndIdx = tokenEndIdx;
        }
    }
}
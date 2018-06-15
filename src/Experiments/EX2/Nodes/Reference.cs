﻿using Liu233w.Compiler.CompilerFramework.Tokenizer;

namespace Liu233w.Compiler.EX2.Nodes
{
    public class Reference : NodeBase
    {
        public PackageName PackageName { get; set; }
        
        public Token Identifier { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.IO;

namespace MonadicParserCombinator
{
    public class Input
    {
        readonly string _source;
        public int _position;
        public int _line;
        public int _column;
        public char Current { get {return _source[_position];} }
        public (int,int,int) Position {get {return (_position, _line, _column);}}
        public bool AtEnd { get { return _position == _source.Length; } }

        public Input(string source)
        {
            _source = source;
            _position = 0;
            _line = 0;
            _column = 0;
        }

        Input(string source, (int,int,int) pos)
        {
            _source = source;
            (_position, _line, _column) = pos;
        }

        public Input Next()
        {
            if (_position == _source.Length) throw new ArgumentOutOfRangeException();

            switch (Current)
            {
                case '\n': return new Input(_source, (_position+1,_line+1,0));
                default  : return new Input(_source, (_position+1,_line,_column+1));

            }
        }
    }
}
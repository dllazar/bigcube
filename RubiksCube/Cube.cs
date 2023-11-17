using System.Text;

namespace RubiksCube
{
    public class Cube
    {
        private readonly string _luaInstanceVarName;
        private string _lastScramble = string.Empty;
        private readonly uint _size;
        private Cube()
        {
            _luaInstanceVarName = $"cube_{GetHashCode()}";
            LuaRuntime.Instance.DoFile($"lua{Path.DirectorySeparatorChar}Lubiks.lua");
            var init = @"
                Lubiks = require(""Lubiks"")
                CubePrint = require(""Lubiks.CubePrint"")
            ";
            LuaRuntime.Instance.DoString(init);
        }
        public Cube(uint size = 3) : this()
        {
            _size = size;  
            LuaRuntime.Instance.DoString($"{_luaInstanceVarName} = Lubiks:new({size})");
        }
        public uint GetSize()
        {
            return _size;
        }

        public string GetFace(Face face)
        {
            var luaSnippet = $"local tab = (\" \"):rep({_luaInstanceVarName}:getSize() * 2)";
            luaSnippet += $"retval = CubePrint:faceToString({_luaInstanceVarName}:getFace(\"{face}\"), tab)";
            LuaRuntime.Instance.DoString(luaSnippet);
            return LuaRuntime.Instance.Globals.Get("retval").CastToString().Replace(" ", "");
        }
        public Dictionary<Face, string> GetFaces()
        {
            var faces = new Dictionary<Face, string>();
            foreach (var face in Enum.GetValues<Face>())
            {
                faces.Add(face, GetFace(face));
            }
            return faces;
        }

        public void SetFace(Face face, string state)
        {
            string cleanState = state.Replace("\n", "").Replace("\r", "").Replace(" ", "");
            var cubeSize = GetSize();
            if (cleanState.Length != (cubeSize * cubeSize)) throw new ArgumentException(nameof(state));
            var builder = new StringBuilder();
            builder.Append("{");
            for (var y = 0; y < cubeSize; y++)
            {
                builder.Append("{");
                for (var x = 0; x < cubeSize; x++)
                {
                    builder.Append($"'{cleanState[(y * (int)cubeSize) + x]}'");
                    if (x < cubeSize - 1)
                    {
                        builder.Append(",");
                    }
                }
                builder.Append("}");
                if (y < cubeSize - 1)
                {
                    builder.Append(",");
                }
            }
            builder.Append("}");
            LuaRuntime.Instance.DoString($"{_luaInstanceVarName}:setFace(\"{face}\", {builder})");
        }
        public void SetFaces(Dictionary<Face, string> faceState)
        {
            foreach (var face in faceState.Keys)
            {
                SetFace(face, faceState[face]);
            }
        }
        public string GetLastScramble() { return _lastScramble; }
        public string GenerateScramble(uint len)
        {
            LuaRuntime.Instance.DoString($"retval= {_luaInstanceVarName}:generateScramble({len})");
            var scramble = LuaRuntime.Instance.Globals.Get("retval").CastToString();
            _lastScramble = scramble;
            return scramble;
        }
        public string Scramble(uint len, bool resetCubeFirst = true, bool keepScrambleMovesInMoveHistory=false)
        {
            if (resetCubeFirst)
            {
                Reset();
            }
            var scramble = GenerateScramble(len);
            Move(scramble);
            if (!keepScrambleMovesInMoveHistory)
            {
                ClearMoveHistory();
            }
            _lastScramble = scramble;
            return scramble;
        }

        public void Move(string movements)
        {
            Console.WriteLine(movements);
            LuaRuntime.Instance.DoString($"retval= {_luaInstanceVarName}:move(\"{movements}\")");
        }
        public void MoveOnce(string move, bool blockHistory = false)
        {
            LuaRuntime.Instance.DoString($"retval= {_luaInstanceVarName}:moveOnce(\"{move}\", {blockHistory})");
        }
        public void UndoMove(uint count = 1)
        {
            LuaRuntime.Instance.DoString($"retval= {_luaInstanceVarName}:undoMove({count})");
        }

        public IEnumerable<string> GetMoveHistory()
        {
            var toReturn = new List<string>();
            var res = LuaRuntime.Instance.DoString($"retval= {_luaInstanceVarName}:getMoveHistory()");
            var tableDict = res.Table.Pairs.ToList();
            foreach (var item in tableDict)
            {
                toReturn.Add(item.Value.CastToString());
            }
            return toReturn;
        }
        public void ClearMoveHistory()
        {
            LuaRuntime.Instance.DoString($"{_luaInstanceVarName}:clearMoveHistory()");
        }

        public bool IsSolved()
        {
            LuaRuntime.Instance.DoString($"retval= {_luaInstanceVarName}:solved()");
            return bool.Parse(LuaRuntime.Instance.Globals.Get("retval").CastToString());
        }

        public void Reset()
        {
            LuaRuntime.Instance.DoString($"{_luaInstanceVarName}:reset()");
        }

        public override String ToString()
        {
            LuaRuntime.Instance.DoString($"retval= {_luaInstanceVarName}:__tostring()");
            return LuaRuntime.Instance.Globals.Get("retval").CastToString();
        }
    }
}
using System.Collections.Generic;
using UserInterface;
using Xunit;

namespace ChessEngine.Test
{
    public class BoardRulesTests
    {
        [Fact]
        public void TestPosition5Depth3()
        {
            var board = new Board(Colour.White, "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8");
            var result = PositionEnumeratorService.GetNumberOfPositions(board, 3);
            var expectedPositions = "a2a3: 1373,a2a4: 1433,b1a3: 1303,b1c3: 1467,b1d2: 1174,b2b3: 1368,b2b4: 1398,c1d2: 1368,c1e3: 1587,c1f4: 1552,c1g5: 1422,c1h6: 1312,c2c3: 1440,c4a6: 1256,c4b3: 1275,c4b5: 1332,c4d3: 1269,c4d5: 1375,c4e6: 1438,c4f7: 1328,d1d2: 1436,d1d3: 1685,d1d4: 1751,d1d5: 1688,d1d6: 1500,d7c8b: 1668,d7c8n: 1607,d7c8q: 1459,d7c8r: 1296,e1d2: 978,e1f1: 1445,e1f2: 1269,e1g1: 1376,e2c3: 1595,e2d4: 1554,e2f4: 1555,e2g1: 1431,e2g3: 1523,g2g3: 1308,g2g4: 1337,h1f1: 1364,h1g1: 1311,h2h3: 1371,h2h4: 1402";
            Assert.Equal(expectedPositions, result.Item2);
            Assert.Equal(62379, result.Item1);
        }

        [Fact]
        public void TestPosition5Depth4()
        {
            var board = new Board(Colour.White, "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8");
            var result = PositionEnumeratorService.GetNumberOfPositions(board, 4);
            var expectedPositions = "a2a3: 46833,a2a4: 48882,b1a3: 44378,b1c3: 50303,b1d2: 40560,b2b3: 46497,b2b4: 46696,c1d2: 46881,c1e3: 53637,c1f4: 52350,c1g5: 45601,c1h6: 40913,c2c3: 49406,c4a6: 41884,c4b3: 43453,c4b5: 45559,c4d3: 43565,c4d5: 48002,c4e6: 49872,c4f7: 43289,d1d2: 48843,d1d3: 57153,d1d4: 57744,d1d5: 56899,d1d6: 43766,d7c8b: 65053,d7c8n: 62009,d7c8q: 44226,d7c8r: 38077,e1d2: 33423,e1f1: 49775,e1f2: 36783,e1g1: 47054,e2c3: 54792,e2d4: 52109,e2f4: 51127,e2g1: 48844,e2g3: 51892,g2g3: 44509,g2g4: 45506,h1f1: 46101,h1g1: 44668,h2h3: 46762,h2h4: 47811";
            Assert.Equal(expectedPositions, result.Item2);
            Assert.Equal(2103487, result.Item1);
        }

        [Fact]
        public void TestStartPositionDepth4()
        {
            var board = new Board(Colour.White);
            var result = PositionEnumeratorService.GetNumberOfPositions(board, 4);
            var expectedPositions = "a2a3: 8457,a2a4: 9329,b1a3: 8885,b1c3: 9755,b2b3: 9345,b2b4: 9332,c2c3: 9272,c2c4: 9744,d2d3: 11959,d2d4: 12435,e2e3: 13134,e2e4: 13160,f2f3: 8457,f2f4: 8929,g1f3: 9748,g1h3: 8881,g2g3: 9345,g2g4: 9328,h2h3: 8457,h2h4: 9329";
            Assert.Equal(expectedPositions, result.Item2);
            Assert.Equal(197281, result.Item1);
        }
    }
}

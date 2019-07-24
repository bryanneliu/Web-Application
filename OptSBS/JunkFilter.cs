using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecalculateLDCG
{
    public class JunkFilter
    {

        internal static void TrimResult(Dictionary<string, List<ResultMetadata>> resultSet)
        {
            foreach (var resultList in resultSet.Values)
            {
                if (resultList.Count > 5)
                {
                    resultList.RemoveRange(5, resultList.Count - 5);
                }
            }
        }

        internal static void DemoteJunk(string type, List<QueryMetadata> querySet, Dictionary<string, List<ResultMetadata>> resultSet)
        {
            foreach (var resultList in resultSet.Values)
            {
                if (resultList.Count >= 4)
                {
                    var reviewScoreList = resultList.Select(s => s.HasReview ? 1 : 0).ToList();
                    var reviewScoreList5 = reviewScoreList.Where((number, index) => index < 5).ToList();
                    var junkScoreList = resultList.Select(s => s.IsNoJunk ? 1 : 0).ToList();
                    var junkScoreList5 = junkScoreList.Where((number, index) => index < 5).ToList();

                    var reviewRate = reviewScoreList.Average();
                    var reviewRate5 = reviewScoreList5.Average();

                    if (resultList.Count > 5 && reviewRate5 < 1 && reviewRate5 > 0.5 && (type == "removejunk2" || type == "removejunk3") )
                    {
                        var targetJunkId = junkScoreList5.FindLastIndex(s => s == 0);
                        if (targetJunkId == -1)
                        {
                            targetJunkId = reviewScoreList5.FindLastIndex(s => s == 0);
                        }
                        int nextGoodId = -1;
                        nextGoodId = reviewScoreList.FindIndex(5, (s => s == 1));
                        if (nextGoodId > 0)
                        {
                            if (type == "removejunk2" || (type == "removejunk3" && QualifyForSwap(resultList, targetJunkId, nextGoodId)))
                            {
                                PostProcessing(resultList, targetJunkId, nextGoodId);
                                Console.WriteLine(resultList.First().EntityGuid);
                            }
                        }
                    }

                    junkScoreList = resultList.Select(s => s.IsNoJunk ? 1 : 0).ToList();
                    junkScoreList5 = junkScoreList.Where((number, index) => index < 5).ToList();
                    //                    var reviewScore = resultList.Select(s => (s.ReviewCount > 3 || s.Rating > 0) ? 1 : 0).ToList();

                    var nojunkRate = junkScoreList.Average();
                    var noJunkRate5 = junkScoreList5.Average();

                    if (noJunkRate5 == 1)
                    {
                        continue;
                    }

                    if (((type == "removejunk0") && noJunkRate5 >= 0.75) || 
                        ((type == "removejunk1" || type == "removejunk2" || type == "removejunk3") && (noJunkRate5 >= 0.75 || (noJunkRate5 == 0.6 && junkScoreList.Count >= 5))))
                    {
                        var targetJunkId = junkScoreList5.FindLastIndex(s => s == 0);
                        int nextGoodId = -1;
                        if (resultList.Count > 5)
                        {
                            nextGoodId = junkScoreList.FindIndex(5, (s => s == 1));
                        }


                        if (type == "removejunk3" && nextGoodId > 0 && !QualifyForSwap(resultList, targetJunkId, nextGoodId))
                        {
                            continue;
                        }

                        PostProcessing(resultList, targetJunkId, nextGoodId);
                    }
                }
            }
        }

        private static bool QualifyForSwap(List<ResultMetadata> resultList, int targetJunkId, int nextGoodId)
        {
            if (resultList[nextGoodId].LineDistance <= 10)
            {
                return true;
            }

            var ratio = resultList[nextGoodId].LineDistance / resultList[targetJunkId].LineDistance;
            if (ratio < 3)
            {
                return true;
            }

            return false;
        }

        private static void PostProcessing(List<ResultMetadata> resultList, int targetJunkId, int nextGoodId)
        {
            if (targetJunkId >= 0)
            {
                if (nextGoodId > 0)
                {
                    // swap 
                    var temp = resultList[targetJunkId];
                    resultList[targetJunkId] = resultList[nextGoodId];
                    resultList[nextGoodId] = temp;
                    resultList.Insert(5, resultList[targetJunkId]);
                    resultList.RemoveAt(targetJunkId);
                }
                else
                {
                    /*
                    // remove
                    for (int i = junkScoreList.Count - 1; i >= 0; i--)
                    {
                        if (junkScoreList[i] == 0)
                            resultList.RemoveAt(i);
                    }
                    */
                    resultList.RemoveAt(targetJunkId);
                }

                for (int i = 0; i < resultList.Count; i++)
                {
                    resultList[i].UpdatePostion(i + 1);
                }
            }
        }
    }
}

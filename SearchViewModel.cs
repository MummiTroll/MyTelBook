using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TelBook
{
    public class SearchViewModel
    {
        public MainViewModel MainViewModel { get; set; }
        public List<string> searchResults = new List<string>();
        public SearchViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
        }
        public void Search()
        {
            searchResults.Clear();
            List<string> keyList = new List<string>(MainViewModel.DBViewModel.DB.Keys);
            List<string> intermediate = new List<string>();
            intermediate = Direct_Search(keyList);
            if (intermediate.Count() >= MainViewModel.HitsItem)
            {
                foreach (string str in intermediate)
                {
                    if (searchResults.Count() < MainViewModel.HitsItem)
                    {
                        if (!searchResults.Contains(str))
                        {
                            searchResults.Add(str);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (intermediate.Count() < MainViewModel.HitsItem)
            {
                foreach (string str in intermediate)
                {
                    if (searchResults.Count() < MainViewModel.HitsItem)
                    {
                        if (!searchResults.Contains(str))
                        {
                            searchResults.Add(str);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                intermediate = Extended_Search(keyList);
                if (intermediate.Count() >= MainViewModel.HitsItem)
                {
                    foreach (string str in intermediate)
                    {
                        if (searchResults.Count() < MainViewModel.HitsItem)
                        {
                            if (!searchResults.Contains(str))
                            {
                                searchResults.Add(str);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (intermediate.Count() < MainViewModel.HitsItem)
                {
                    intermediate = Extended_Search(keyList);
                    foreach (string str in intermediate)
                    {
                        if (searchResults.Count() < MainViewModel.HitsItem)
                        {
                            if (!searchResults.Contains(str))
                            {
                                searchResults.Add(str);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        public List<string> Direct_Search(List<string> keyList)
        {
            List<string> results = new List<string>();
            for (int i = 0; i < keyList.Count(); i++)
            {
                List<string> hits = new List<string>();
                for (int j = 0; j < MainViewModel.KeywordList.Count(); j++)
                {
                    for (int k = 0; k < MainViewModel.DBViewModel.DB[keyList[i]].Count(); k++)
                    {
                        for (int l = 0; l < MainViewModel.DBViewModel.DB[keyList[i]][k].Count(); l++)
                        {
                            if (MainViewModel.DBViewModel.DB[keyList[i]][k][l].ToLower().Contains(MainViewModel.KeywordList[j].ToLower()) && MainViewModel.All_InOne == true)
                            {
                                if (!hits.Contains(MainViewModel.KeywordList[j]))
                                {
                                    hits.Add(MainViewModel.KeywordList[j]);
                                }
                            }
                            else if (MainViewModel.DBViewModel.DB[keyList[i]][k][l].ToLower().Contains(MainViewModel.KeywordList[j].ToLower()) && MainViewModel.All_InOne == false)
                            {
                                if (!results.Contains(keyList[i]))
                                {
                                    results.Add(keyList[i]);
                                }
                            }
                        }
                    }
                }
                if (MainViewModel.All_InOne == true && hits.Count() >= MainViewModel.KeywordList.Count())
                {
                    if (!results.Contains(keyList[i]))
                    {
                        results.Add(keyList[i]);
                    }
                }
            }
            return results;
        }
        public List<string> Extended_Search(List<string> keyList)
        {
            List<string> results = new List<string>();
            for (int i = 0; i < keyList.Count(); i++)
            {
                List<string> hits = new List<string>();
                for (int j = 0; j < MainViewModel.KeywordList.Count(); j++)
                {
                    string keyword = MainViewModel.KeywordList[j];
                    string keywordLeft = keyword;
                    string keywordRight = keyword;
                    if (keyword.Length >= MainViewModel.MinWindow)
                    {
                        for (int k = 0; k < keyword.Length - MainViewModel.MinWindow; k++)
                        {
                            keywordLeft = keyword.Substring(k + 1);
                            keywordRight = keyword.Substring(0, keyword.Length - (k + 1));
                            for (int l = 0; l < MainViewModel.DBViewModel.DB[keyList[i]].Count(); l++)
                            {
                                for (int m = 0; m < MainViewModel.DBViewModel.DB[keyList[i]][l].Count(); m++)
                                {
                                    if (MainViewModel.DBViewModel.DB[keyList[i]][l][m].ToLower().Contains(keywordLeft.ToLower())
                                        || MainViewModel.DBViewModel.DB[keyList[i]][l][m].ToLower().Contains(keywordRight.ToLower()))
                                    {
                                        if (!hits.Contains(keyword))
                                        {
                                            hits.Add(keyword);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (MainViewModel.All_InOne == true && hits.Count() >= MainViewModel.KeywordList.Count())
                {
                    if (!results.Contains(keyList[i]))
                    {
                        results.Add(keyList[i]);
                    }
                }
                else if (MainViewModel.All_InOne == false && hits.Count() > 0)
                {
                    if (!results.Contains(keyList[i]))
                    {
                        results.Add(keyList[i]);
                    }
                }
            }
            return results;
        }
    }
}

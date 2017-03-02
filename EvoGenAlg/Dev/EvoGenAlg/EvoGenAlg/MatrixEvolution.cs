using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoGenAlg
{
    //
    //http://www.bbc.com/news/uk-35058761
    //
    public class MatrixEvolution : IEvolution
    {
        List<List<int>> _mat;
        int _size;
        int _maxVariations = 50;
        int _fitness = 0;
        static Random _rndNumber = new Random((int)DateTime.Now.Ticks);
        static List<List<int>> _rows;
        static List<List<int>> _cols;
        static int _nDesiredFitness = 0;
        static int _nMaxFitnessUntilNow = 0;
        List<List<string>> _HorizontalVariations = new List<List<string>>();
        List<List<string>> _VerticalVariations = new List<List<string>>();

        public MatrixEvolution(int size, List<List<int>> mat)
        {
            _size = size;
            _mat = new List<List<int>>();
            for (int i = 0; i < _size; i++)
            {
                List<int> emptyRow = new List<int> { };
                for (int j = 0; j < _size; j++)
                {
                    emptyRow.Add(mat[i][j]);
                }
                _mat.Add(emptyRow);
            }
        }

        public MatrixEvolution()
        {
            _size = 25;
            _mat = new List<List<int>>();
            _rows = new List<List<int>>();
            _cols = new List<List<int>>();
        }

        List<List<int>> Mat
        {
            get { return _mat; }
            set { _mat = value; }
        }

        public int DesiredFitness
        {
            get
            {
                if (_nDesiredFitness == 0)
                {
                    int nRowDotSum = 0;
                    foreach (List<int> items in _rows)
                    {
                        nRowDotSum += items.Sum();
                    }

                    int nColDotSum = 0;
                    foreach (List<int> items in _cols)
                    {
                        nColDotSum += items.Sum();
                    }

                    if (nRowDotSum != nColDotSum)
                    {
                        Debug.Assert(false, "Wrong initialization");
                    }

                    _nDesiredFitness = nRowDotSum + nColDotSum;
                }

                return _nDesiredFitness;
            }
        }

        public void Evolve()
        {
        }

        public int Fitness
        {
            get
            {
                if (_fitness == 0)
                {
                    try
                    {
                        for (int i = 0; i < _size; i++)
                        {
                            int nRowSum = 0;
                            int nColSum = 0;
                            int nHorizConsecutiveCount = 0;
                            int nHorizBlockCount = 0;
                            int nVertConsecutiveCount = 0;
                            int nVertBlockCount = 0;
                            for (int j = 0; j < _size; j++)
                            {
                                if (Math.Abs(_mat[i][j]) == 1)
                                {
                                    if (nHorizConsecutiveCount == 0)
                                    {
                                        nHorizBlockCount++;
                                    }
                                    nHorizConsecutiveCount++;
                                }
                                if (Math.Abs(_mat[i][j]) != 1 || j == _size - 1)
                                {
                                    if (nHorizConsecutiveCount > 0)
                                    {
                                        if (_rows[i].Count > nHorizBlockCount - 1)
                                        {
                                            if (nHorizConsecutiveCount == _rows[i][nHorizBlockCount - 1])
                                            {
                                                nRowSum += nHorizConsecutiveCount;
                                            }
                                        }
                                        else
                                        {
                                            // nRowSum -= nHorizConsecutiveCount;
                                        }
                                        nHorizConsecutiveCount = 0;
                                    }
                                }

                                if (Math.Abs(_mat[j][i]) == 1)
                                {
                                    if (nVertConsecutiveCount == 0)
                                    {
                                        nVertBlockCount++;
                                    }
                                    nVertConsecutiveCount++;
                                }
                                if (Math.Abs(_mat[j][i]) != 1 || j == _size - 1)
                                {
                                    if (nVertConsecutiveCount > 0)
                                    {
                                        if (_cols[i].Count > nVertBlockCount - 1)
                                        {
                                            if (nVertConsecutiveCount == _cols[i][nVertBlockCount - 1])
                                            {
                                                nColSum += nVertConsecutiveCount;
                                            }
                                        }
                                        else
                                        {
                                            //nColSum -= nVertConsecutiveCount;
                                        }
                                        nVertConsecutiveCount = 0;
                                    }
                                }
                            }

                            _fitness += nRowSum;
                            _fitness += nColSum;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Assert(false);
                    }
                }

                return _fitness;
            }
            set
            {
                _fitness = value;
            }
        }

        public int GetHorizontalFitness(int nRow)
        {
            int nRowFitness = 0;

            int nHorizConsecutiveCount = 0;
            int nHorizBlockCount = 0;
            for (int j = 0; j < _size; j++)
            {
                if (Math.Abs(_mat[nRow][j]) == 1)
                {
                    if (nHorizConsecutiveCount == 0)
                    {
                        nHorizBlockCount++;
                    }
                    nHorizConsecutiveCount++;
                }
                if (Math.Abs(_mat[nRow][j]) != 1 || j == _size - 1)
                {
                    if (nHorizConsecutiveCount > 0)
                    {
                        if (_rows[nRow].Count > nHorizBlockCount - 1)
                        {
                            if (nHorizConsecutiveCount == _rows[nRow][nHorizBlockCount - 1])
                            {
                                nRowFitness += nHorizConsecutiveCount;
                            }
                        }
                        else
                        {
                            nRowFitness = -1;
                            break;
                        }
                        nHorizConsecutiveCount = 0;
                    }
                }
            }

            return nRowFitness;
        }

        public int GetVerticalFitness(int nCol)
        {
            int nColFitness = 0;

            int nVertConsecutiveCount = 0;
            int nVertBlockCount = 0;

            for (int j = 0; j < _size; j++)
            {
                if (Math.Abs(_mat[j][nCol]) == 1)
                {
                    if (nVertConsecutiveCount == 0)
                    {
                        nVertBlockCount++;
                    }
                    nVertConsecutiveCount++;
                }
                if (Math.Abs(_mat[j][nCol]) != 1 || j == _size - 1)
                {
                    if (nVertConsecutiveCount > 0)
                    {
                        if (_cols[nCol].Count > nVertBlockCount - 1)
                        {
                            if (nVertConsecutiveCount == _cols[nCol][nVertBlockCount - 1])
                            {
                                nColFitness += nVertConsecutiveCount;
                            }
                        }
                        else
                        {
                            nColFitness = -1;
                            break;
                        }
                        nVertConsecutiveCount = 0;
                    }
                }
            }

            return nColFitness;
        }

        public int GetDesiredHorizontalFitness(int nRow)
        {
            return _rows[nRow].Sum();
        }

        public int GetDesiredVerticalFitness(int nCol)
        {
            return _cols[nCol].Sum();
        }

        public object Data
        {
            get
            {
                return Mat;
            }
            set
            {

            }
        }

        public void Initialize()
        {
            for (int j = 0; j < _size; j++)
            {
                List<int> emptyRow = new List<int> { };
                for (int i = 0; i < _size; i++)
                {
                    emptyRow.Add(0);
                }
                _mat.Add(emptyRow);
            }

            //rows requirements
            _rows.Add(new List<int> { 7, 3, 1, 1, 7 });
            _rows.Add(new List<int> { 1, 1, 2, 2, 1, 1 });
            _rows.Add(new List<int> { 1, 3, 1, 3, 1, 1, 3, 1 });
            _rows.Add(new List<int> { 1, 3, 1, 1, 6, 1, 3, 1 });
            _rows.Add(new List<int> { 1, 3, 1, 5, 2, 1, 3, 1 });
            _rows.Add(new List<int> { 1, 1, 2, 1, 1 });
            _rows.Add(new List<int> { 7, 1, 1, 1, 1, 1, 7 });
            _rows.Add(new List<int> { 3, 3 });
            _rows.Add(new List<int> { 1, 2, 3, 1, 1, 3, 1, 1, 2 });
            _rows.Add(new List<int> { 1, 1, 3, 2, 1, 1 });
            _rows.Add(new List<int> { 4, 1, 4, 2, 1, 2 });
            _rows.Add(new List<int> { 1, 1, 1, 1, 1, 4, 1, 3 });
            _rows.Add(new List<int> { 2, 1, 1, 1, 2, 5 });
            _rows.Add(new List<int> { 3, 2, 2, 6, 3, 1 });
            _rows.Add(new List<int> { 1, 9, 1, 1, 2, 1 });
            _rows.Add(new List<int> { 2, 1, 2, 2, 3, 1 });
            _rows.Add(new List<int> { 3, 1, 1, 1, 1, 5, 1 });
            _rows.Add(new List<int> { 1, 2, 2, 5 });
            _rows.Add(new List<int> { 7, 1, 2, 1, 1, 1, 3 });
            _rows.Add(new List<int> { 1, 1, 2, 1, 2, 2, 1 });
            _rows.Add(new List<int> { 1, 3, 1, 4, 5, 1 });
            _rows.Add(new List<int> { 1, 3, 1, 3, 10, 2 });
            _rows.Add(new List<int> { 1, 3, 1, 1, 6, 6 });
            _rows.Add(new List<int> { 1, 1, 2, 1, 1, 2 });
            _rows.Add(new List<int> { 7, 2, 1, 2, 5 });

            // requirements
            _cols.Add(new List<int> { 7, 2, 1, 1, 7 });
            _cols.Add(new List<int> { 1, 1, 2, 2, 1, 1, });
            _cols.Add(new List<int> { 1, 3, 1, 3, 1, 3, 1, 3, 1 });
            _cols.Add(new List<int> { 1, 3, 1, 1, 5, 1, 3, 1 });
            _cols.Add(new List<int> { 1, 3, 1, 1, 4, 1, 3, 1 });
            _cols.Add(new List<int> { 1, 1, 1, 2, 1, 1 });
            _cols.Add(new List<int> { 7, 1, 1, 1, 1, 1, 7 });
            _cols.Add(new List<int> { 1, 1, 3 });
            _cols.Add(new List<int> { 2, 1, 2, 1, 8, 2, 1 });
            _cols.Add(new List<int> { 2, 2, 1, 2, 1, 1, 1, 2 });
            _cols.Add(new List<int> { 1, 7, 3, 2, 1 });
            _cols.Add(new List<int> { 1, 2, 3, 1, 1, 1, 1, 1 });
            _cols.Add(new List<int> { 4, 1, 1, 2, 6 });
            _cols.Add(new List<int> { 3, 3, 1, 1, 1, 3, 1 });
            _cols.Add(new List<int> { 1, 2, 5, 2, 2 });
            _cols.Add(new List<int> { 2, 2, 1, 1, 1, 1, 1, 2, 1 });
            _cols.Add(new List<int> { 1, 3, 3, 2, 1, 8, 1 });
            _cols.Add(new List<int> { 6, 2, 1 });
            _cols.Add(new List<int> { 7, 1, 4, 1, 1, 3 });
            _cols.Add(new List<int> { 1, 1, 1, 1, 4 });
            _cols.Add(new List<int> { 1, 3, 1, 3, 7, 1 });
            _cols.Add(new List<int> { 1, 3, 1, 1, 1, 2, 1, 1, 4 });
            _cols.Add(new List<int> { 1, 3, 1, 4, 3, 3 });
            _cols.Add(new List<int> { 1, 1, 2, 2, 2, 6, 1 });
            _cols.Add(new List<int> { 7, 1, 3, 2, 1, 1 });

            /*
            for(int i = 0;i<_size;i++)
            {
                _rows[i].Reverse();
                _cols[i].Reverse();
            }
            */
            /*
            //set the fixed squares
            _mat[3][0] = 1;
            _mat[3][2] = 1;
            _mat[3][3] = 1;// -1;
            _mat[3][4] = 1;// -1;
            _mat[3][6] = 1;
            _mat[3][8] = 1;
            _mat[3][10] = 1;
            _mat[3][11] = 1;
            _mat[3][12] = 1;// -1;
            _mat[3][13] = 1;// -1;
            _mat[3][14] = 1;
            _mat[3][15] = 1;
            _mat[3][17] = 1;
            _mat[3][19] = 1;
            _mat[3][20] = 1;
            _mat[3][21] = 1;// -1;
            _mat[3][23] = 1;

            _mat[8][0] = 1;
            _mat[8][2] = 1;
            _mat[8][3] = 1;
            _mat[8][5] = 1;
            _mat[8][6] = 1;// -1;
            _mat[8][7] = 1;// -1;
            _mat[8][10] = 1;// -1;
            _mat[8][12] = 1;
            _mat[8][14] = 1;// -1;
            _mat[8][15] = 1;// -1;
            _mat[8][16] = 1;
            _mat[8][18] = 1;// -1;
            _mat[8][20] = 1;
            _mat[8][22] = 1;
            _mat[8][23] = 1;

            _mat[16][0] = 1;
            _mat[16][1] = 1;
            _mat[16][2] = 1;
            _mat[16][4] = 1;
            _mat[16][6] = 1;// -1;
            _mat[16][8] = 1;
            _mat[16][11] = 1;// -1;
            _mat[16][13] = 1;
            _mat[16][14] = 1;
            _mat[16][15] = 1;
            _mat[16][16] = 1;// -1;
            _mat[16][17] = 1;
            _mat[16][20] = 1;// -1;

            _mat[21][0] = 1;
            _mat[21][2] = 1;
            _mat[21][3] = 1;// -1;
            _mat[21][4] = 1;// -1;
            _mat[21][6] = 1;
            _mat[21][8] = 1;
            _mat[21][9] = 1;// -1;
            _mat[21][10] = 1;// -1;
            _mat[21][12] = 1;
            _mat[21][13] = 1;
            _mat[21][14] = 1;
            _mat[21][15] = 1;// -1;
            _mat[21][16] = 1;
            _mat[21][17] = 1;
            _mat[21][18] = 1;
            _mat[21][19] = 1;
            _mat[21][20] = 1;// -1;
            _mat[21][21] = 1;// -1;
            _mat[21][23] = 1;
            _mat[21][24] = 1;
            */
            /*
            for (int i = 0; i < _size; i++)
            {
                //if (i != 3 && i != 8 && i != 16 && i != 21)
                {
                    int nCounter = 0;
                    for (int j = 0; j < _rows[i].Count; j++)
                    {
                        int nFilledCels = _rows[i][j];
                        for (int k = 0; k < nFilledCels; k++)
                        {
                            _mat[i][nCounter] = 1;
                            nCounter++;
                        }
                        if (nCounter < _size)
                        {
                            _mat[i][nCounter] = 0;
                            nCounter++;
                        }
                    }
                }
            }*/
        }

        public void ValidateVerticalVariations()
        {
            List<List<string>> newVerticalVariations = new List<List<string>>();

            for (int j = 0; j < _size; j++)
            {
                if (j == 3)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[3] == '1' && col[21] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 6)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[8] == '1' && col[16] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 7)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[8] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 9)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[21] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 10)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[8] == '1' && col[21] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 11)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[16] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 12)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[3] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 13)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[3] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 14)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[8] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 15)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[8] == '1' && col[21] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 16)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[16] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 18)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[8] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 20)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[16] == '1' && col[21] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else if (j == 21)
                {
                    List<string> newCol = new List<string>();
                    foreach (string col in _VerticalVariations[j])
                    {
                        if (col[3] == '1' && col[21] == '1')
                        {
                            newCol.Add(col);
                        }
                    }
                    newVerticalVariations.Add(newCol);
                }
                else
                {
                    newVerticalVariations.Add(_VerticalVariations[j]);
                }
            }
            _VerticalVariations.Clear();
            _VerticalVariations = newVerticalVariations;
        }

        public void ValidateHorizontalVariations()
        {
            List<List<string>> newHorizontalVariations = new List<List<string>>();
            
            for (int j = 0; j < _size; j++)
            {
                if (j == 3)
                {
                    List<string> newRow = new List<string>();
                    foreach (string row in _HorizontalVariations[j])
                    {
                        /*
                        _mat[3][3] = 1;// -1;
                        _mat[3][4] = 1;// -1;
                        _mat[3][12] = 1;// -1;
                        _mat[3][13] = 1;// -1;
                        _mat[3][21] = 1;// -1;
                        */
                        if (row[3] == '1' && row[4] == '1' && row[12] == '1' && row[13] == '1' && row[21] == '1')
                        {
                            newRow.Add(row);
                        }
                    }
                    newHorizontalVariations.Add(newRow);
                }
                else if (j == 8)
                {
                    /*
            _mat[8][6] = 1;// -1;
            _mat[8][7] = 1;// -1;
            _mat[8][10] = 1;// -1;
            _mat[8][14] = 1;// -1;
            _mat[8][15] = 1;// -1;
            _mat[8][18] = 1;// -1;
                    */
                    List<string> newRow = new List<string>();
                    foreach (string row in _HorizontalVariations[j])
                    {
                        if (row[6] == '1' && row[7] == '1' && row[10] == '1' && row[14] == '1' && row[15] == '1' && row[18] == '1')
                        {
                            newRow.Add(row);
                        }
                    }
                    newHorizontalVariations.Add(newRow);
                }
                else if (j == 16)
                {
                    /*
                    _mat[16][6] = 1;// -1;
            _mat[16][11] = 1;// -1;
            _mat[16][16] = 1;// -1;
            _mat[16][20] = 1;// -1;
                    */
                    List<string> newRow = new List<string>();
                    foreach (string row in _HorizontalVariations[j])
                    {
                        if (row[6] == '1' && row[11] == '1' && row[16] == '1' && row[20] == '1')
                        {
                            newRow.Add(row);
                        }
                    }
                    newHorizontalVariations.Add(newRow);
                }
                else if (j == 21)
                {
                    /*
            _mat[21][3] = 1;// -1;
            _mat[21][4] = 1;// -1;
            _mat[21][9] = 1;// -1;
            _mat[21][10] = 1;// -1;
            _mat[21][15] = 1;// -1;
            _mat[21][20] = 1;// -1;
            _mat[21][21] = 1;// -1;
                    */
                    List<string> newRow = new List<string>();
                    foreach (string row in _HorizontalVariations[j])
                    {
                        if (row[3] == '1' && row[4] == '1' && row[9] == '1' && row[10] == '1' && row[15] == '1' && row[20] == '1' && row[21] == '1')
                        {
                            newRow.Add(row);
                        }
                    }
                    newHorizontalVariations.Add(newRow);
                }
                else
                {
                    newHorizontalVariations.Add(_HorizontalVariations[j]);
                }
            }
            _HorizontalVariations.Clear();
            _HorizontalVariations = newHorizontalVariations;
        }

        public IEvolution Mutate()
        {
            MatrixEvolution matEvo = new MatrixEvolution(_size, _mat);

            //matEvo.GetAllHorizontalVariations();
            //matEvo.GetAllVerticalVariations();

            matEvo.InitializeVariations();

            int nTotalHorizVariations = 0;
            int nTotalVertVariations = 0;
            for (int i = 0; i < _size; i++)
            {
                nTotalHorizVariations += matEvo._HorizontalVariations[i].Count;
                nTotalVertVariations += matEvo._VerticalVariations[i].Count;
            }
            int nTotalVariations = nTotalVertVariations + nTotalHorizVariations;

            matEvo.ValidateHorizontalVariations();
            matEvo.ValidateVerticalVariations();

            int nCount = 0;
            while (true)
            {
                matEvo.FilterHorizontalVariations();
                matEvo.FilterVerticalVariations();

                nTotalHorizVariations = 0;
                nTotalVertVariations = 0;
                for (int i = 0; i < _size; i++)
                {
                    nTotalHorizVariations += matEvo._HorizontalVariations[i].Count;
                    nTotalVertVariations += matEvo._VerticalVariations[i].Count;
                }
                if (nTotalVariations > nTotalHorizVariations+ nTotalVertVariations)
                {
                    nTotalVariations = nTotalHorizVariations + nTotalVertVariations;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < _size; i++)
            {
                SerializeList(i, matEvo._HorizontalVariations[i], true);
            }

            for (int i = 0; i < _size; i++)
            {
                SerializeList(i, matEvo._VerticalVariations[i], false);
            }

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if(matEvo._HorizontalVariations[i][0][j] == '1')
                    {
                        matEvo.Mat[i][j] = 1;
                    }
                    else
                    {
                        matEvo.Mat[i][j] = 0;
                    }
                }
            }
            return matEvo;
        }

        private int FilterHorizontalVariations()
        {
            int nRet = 0;
            for(int i=0; i<_size;i++)
            {
                if(_VerticalVariations[i].Count == 1)
                {
                    nRet++;
                    List<List<string>> newHorizontalVariations = new List<List<string>>();
                    string col = _VerticalVariations[i][0];
                    for (int j = 0; j < _size; j++)
                    {
                        List<string> newRow = new List<string>();
                        foreach (string row in _HorizontalVariations[j])
                        {
                            if(row[i] == col[j])
                            {
                                newRow.Add(row);
                            }
                        }
                        newHorizontalVariations.Add(newRow);
                    }
                    _HorizontalVariations.Clear();
                    _HorizontalVariations = newHorizontalVariations;
                }
                else 
                {
                    nRet++;
                    List<List<string>> newHorizontalVariations = new List<List<string>>();
                    string col1 = _VerticalVariations[i][0];
                    
                    for (int j = 0; j < _size; j++)
                    {
                        List<string> newRow = new List<string>();
                        foreach (string row in _HorizontalVariations[j])
                        {
                            bool bAllEqual = true;
                            for(int k=1; k<_VerticalVariations[i].Count;k++)
                            {
                                if(col1[j] != _VerticalVariations[i][k][j])
                                {
                                    bAllEqual = false;
                                    break;
                                }
                            }
                            if (bAllEqual)
                            {
                                if (row[i] == col1[j])
                                {
                                    newRow.Add(row);
                                }
                            }
                            else
                            {
                                newRow.Add(row);
                            }
                        }
                        newHorizontalVariations.Add(newRow);
                    }
                    _HorizontalVariations.Clear();
                    _HorizontalVariations = newHorizontalVariations;
                }
            }

            return nRet;
        }

        private int FilterVerticalVariations()
        {
            int nRet = 0;
            for (int i = 0; i < _size; i++)
            {
                if (_HorizontalVariations[i].Count == 1)
                {
                    nRet++;
                    List<List<string>> newVerticalVariations = new List<List<string>>();
                    string row = _HorizontalVariations[i][0];
                    for (int j = 0; j < _size; j++)
                    {
                        List<string> newCol = new List<string>();
                        foreach (string col in _VerticalVariations[j])
                        {
                            if (col[i] == row[j])
                            {
                                newCol.Add(col);
                            }
                        }
                        newVerticalVariations.Add(newCol);
                    }
                    _VerticalVariations.Clear();
                    _VerticalVariations = newVerticalVariations;
                }
                if (_HorizontalVariations[i].Count == 2)
                {
                    nRet++;
                    List<List<string>> newVerticalVariations = new List<List<string>>();
                    string row1 = _HorizontalVariations[i][0];
                    for (int j = 0; j < _size; j++)
                    {
                        List<string> newCol = new List<string>();
                        foreach (string col in _VerticalVariations[j])
                        {
                            bool bAllEqual = true;
                            for (int k = 1; k < _HorizontalVariations[i].Count; k++)
                            {
                                if (row1[j] != _HorizontalVariations[i][k][j])
                                {
                                    bAllEqual = false;
                                    break;
                                }
                            }
                            if (bAllEqual)
                            {
                                if (col[i] == row1[j])
                                {
                                    newCol.Add(col);
                                }
                            }
                            else
                            {
                                newCol.Add(col);
                            }
                        }
                        newVerticalVariations.Add(newCol);
                    }
                    _VerticalVariations.Clear();
                    _VerticalVariations = newVerticalVariations;
                }
            }

            return nRet;
        }

        public IEvolution GetAllHorizontalVariations()
        {
            MatrixEvolution matEvo = new MatrixEvolution(_size, _mat);

            DateTime dt = DateTime.Now;
            List<string> validCombinations = new List<string>();
            int nIndex = 0;
            bool bFoundIt = false;
            bool bAllMovesDoneForCurrentBlock = false;
            int nMaxIndexUntilNow = 0;
            int nGeneralRowIndex = 0;
            while (!bFoundIt)
            {
                if (nIndex + 1 == _size)
                {
                    SerializeList(nGeneralRowIndex, validCombinations, true);
                    _HorizontalVariations.Add(validCombinations);
                    validCombinations = new List<string>();
                    nGeneralRowIndex++;
                    bAllMovesDoneForCurrentBlock = false;
                    nIndex = 0;
                }

                if (nGeneralRowIndex == _size)
                {
                    break;
                }

                bool bTmp = bAllMovesDoneForCurrentBlock;
                if (bAllMovesDoneForCurrentBlock)
                {
                    nIndex++;
                }


                int nRow = nIndex / _size + nGeneralRowIndex;
                int nCol = nIndex % _size;
                int nVal = matEvo.Mat[nRow][nCol];

                matEvo.Mat[nRow][nCol] = (nVal + 1) % 2;

                if (matEvo.GetHorizontalFitness(nRow) == matEvo.GetDesiredHorizontalFitness(nRow))
                {
                    string s = matEvo.GetRowAsString(nRow);
                    if (!validCombinations.Contains(s))
                    {
                        validCombinations.Add(s);
                    }
                }

                if (nMaxIndexUntilNow < nIndex)
                {
                    nMaxIndexUntilNow = nIndex;
                }

                bAllMovesDoneForCurrentBlock = (nVal == 1);


                if (bTmp && !bAllMovesDoneForCurrentBlock)
                {
                    nIndex = 0;
                }
            }

            return matEvo;
        }

        public IEvolution GetAllVerticalVariations()
        {
            MatrixEvolution matEvo = new MatrixEvolution(_size, _mat);

            DateTime dt = DateTime.Now;
            List<string> validCombinations = new List<string>();
            int nIndex = 0;
            bool bFoundIt = false;
            bool bAllMovesDoneForCurrentBlock = false;
            int nMaxIndexUntilNow = 0;
            int nGeneralColIndex = 0;
            while (!bFoundIt)
            {
                if (nIndex + 1 == _size)
                {
                    SerializeList(nGeneralColIndex, validCombinations, false);
                    _VerticalVariations.Add(validCombinations);
                    validCombinations = new List<string>();
                    nGeneralColIndex++;
                    bAllMovesDoneForCurrentBlock = false;
                    nIndex = 0;
                }

                if (nGeneralColIndex == _size)
                {
                    break;
                }

                bool bTmp = bAllMovesDoneForCurrentBlock;
                if (bAllMovesDoneForCurrentBlock)
                {
                    nIndex++;
                }


                int nCol = nIndex / _size + nGeneralColIndex;
                int nRow = nIndex % _size;
                int nVal = matEvo.Mat[nRow][nCol];

                matEvo.Mat[nRow][nCol] = (nVal + 1) % 2;

                if (matEvo.GetVerticalFitness(nCol) == matEvo.GetDesiredVerticalFitness(nCol))
                {
                    string s = matEvo.GetColAsString(nCol);
                    if (!validCombinations.Contains(s))
                    {
                        validCombinations.Add(s);
                    }
                }

                if (nMaxIndexUntilNow < nIndex)
                {
                    nMaxIndexUntilNow = nIndex;
                }

                bAllMovesDoneForCurrentBlock = (nVal == 1);


                if (bTmp && !bAllMovesDoneForCurrentBlock)
                {
                    nIndex = 0;
                }
            }

            double dMinutes = (DateTime.Now - dt).TotalMinutes;



            return matEvo;
        }

        public IEvolution Mutate1()
        {
            MatrixEvolution matEvo = new MatrixEvolution(_size, _mat);


            int nGeneralBlockIndex = 0;
            bool bFoundIt = false;
            bool bAllMovesDoneForCurrentBlock = false;
            int nMoveLeft = 1;

            while (!bFoundIt)
            {
                bool bTmp = bAllMovesDoneForCurrentBlock;
                if (bAllMovesDoneForCurrentBlock)
                {
                    nGeneralBlockIndex++;
                    nMoveLeft = 1;
                }
                bFoundIt = matEvo.PerformNextMove(nGeneralBlockIndex, ref bAllMovesDoneForCurrentBlock, ref nMoveLeft);
                if (bTmp && !bAllMovesDoneForCurrentBlock)
                {
                    nGeneralBlockIndex = 0;
                }

            }

            return matEvo;
        }

        public bool PerformNextMove(int nGeneralBlockIndex, ref bool bAllMovesDoneForCurrentBlock, ref int nMoveLeft)
        {
            bool bFoundIt = false;
            int nRowIndex = 0;
            int nBlockIndex = 0;
            int nStartBlockIndex = 0;
            int nEndBlockIndex = 0;

            bAllMovesDoneForCurrentBlock = false;

            GetRowAndBlockIndex(nGeneralBlockIndex, out nRowIndex, out nBlockIndex);
            GetBlockStartEndIndex(nRowIndex, nBlockIndex, ref nStartBlockIndex, ref nEndBlockIndex);

            if (nMoveLeft == 1)
            {
                if (MoveBlockLeftRight(nRowIndex, nBlockIndex, nStartBlockIndex, nEndBlockIndex, nMoveLeft))
                {
                    Fitness = 0;
                    if (Fitness == DesiredFitness)
                    {
                        bFoundIt = true;
                    }
                }
                else
                {
                    nMoveLeft = 0;
                }
            }
            if (nMoveLeft == 0)
            {
                if (MoveBlockLeftRight(nRowIndex, nBlockIndex, nStartBlockIndex, nEndBlockIndex, nMoveLeft))
                {
                    Fitness = 0;
                    if (Fitness == DesiredFitness)
                    {
                        bFoundIt = true;
                    }
                }
                else
                {
                    bAllMovesDoneForCurrentBlock = true;
                }
            }


            if (_nMaxFitnessUntilNow < Fitness)
            {
                _nMaxFitnessUntilNow = Fitness;
            }

            return bFoundIt;
        }

        private void GetRowAndBlockIndex(int nGeneralBlockIndex, out int nRowIndex, out int nBlockIndex)
        {
            nRowIndex = 0;
            nBlockIndex = 0;

            int nBlockCounter = 0;
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _rows[i].Count; j++)
                {
                    if (nBlockCounter == nGeneralBlockIndex)
                    {
                        nRowIndex = i;
                        nBlockIndex = j;
                        return;
                    }
                    else
                    {
                        nBlockCounter++;
                    }
                }
            }

            Debug.Assert(false);
        }

        public IEvolution Mutate2()
        {
            MatrixEvolution matEvo = new MatrixEvolution(_size, _mat);
            int vars = _rndNumber.Next(_maxVariations);
            int nCounter = 0;
            while (nCounter < vars)
            {
                int nRowIndex = _rndNumber.Next(_size);
                int nBlockIndex = _rndNumber.Next(_rows[nRowIndex].Count);
                int nMoveToLeft = _rndNumber.Next(2);
                int nStartBlockIndex = 0;
                int nEndBlockIndex = 0;
                matEvo.GetBlockStartEndIndex(nRowIndex, nBlockIndex, ref nStartBlockIndex, ref nEndBlockIndex);

                if (MoveBlockLeftRight(nRowIndex, nBlockIndex, nStartBlockIndex, nEndBlockIndex, nMoveToLeft))
                {
                    nCounter++;
                    matEvo.Fitness = 0;
                    if (matEvo.Fitness > Fitness)
                    {
                        break;
                    }
                }
            }

            return matEvo;
        }

        private bool MoveBlockLeftRight(int nRowIndex, int nBlockIndex, int nStartBlockIndex, int nEndBlockIndex, int nMoveToLeft)
        {
            bool bHasChanged = false;

            if (nMoveToLeft == 1)
            {
                if (nStartBlockIndex == 1)
                {
                    if (_mat[nRowIndex][nEndBlockIndex] != -1)
                    {
                        _mat[nRowIndex][nStartBlockIndex - 1] = 1;
                        _mat[nRowIndex][nEndBlockIndex] = 0;
                        bHasChanged = true;
                    }
                }
                else if (nStartBlockIndex - 2 >= 0)
                {
                    if (_mat[nRowIndex][nEndBlockIndex] != -1 && _mat[nRowIndex][nStartBlockIndex - 2] == 0)
                    {
                        _mat[nRowIndex][nStartBlockIndex - 1] = 1;
                        _mat[nRowIndex][nEndBlockIndex] = 0;
                        bHasChanged = true;
                    }
                }
            }
            else
            {
                if (nEndBlockIndex == _size - 2)
                {
                    if (_mat[nRowIndex][nStartBlockIndex] != -1)
                    {
                        _mat[nRowIndex][nStartBlockIndex] = 0;
                        _mat[nRowIndex][nEndBlockIndex + 1] = 1;
                        bHasChanged = true;
                    }
                }
                else if (nEndBlockIndex + 2 < _size)
                {
                    if (_mat[nRowIndex][nStartBlockIndex] != -1 && _mat[nRowIndex][nEndBlockIndex + 2] == 0)
                    {
                        _mat[nRowIndex][nStartBlockIndex] = 0;
                        _mat[nRowIndex][nEndBlockIndex + 1] = 1;
                        bHasChanged = true;
                    }
                }
            }

            return bHasChanged;
        }

        private void GetBlockStartEndIndex(int nRowIndex, int nBlockIndex, ref int nStartBlockIndex, ref int nEndBlockIndex)
        {
            int nHorizConsecutiveCount = 0;
            int nHorizBlockCount = 0;
            for (int j = 0; j < _size; j++)
            {
                if (Math.Abs(_mat[nRowIndex][j]) == 1)
                {
                    if (nHorizConsecutiveCount == 0)
                    {
                        nHorizBlockCount++;
                    }
                    nHorizConsecutiveCount++;
                }
                if (Math.Abs(_mat[nRowIndex][j]) != 1 || j == _size - 1)
                {
                    if (nHorizConsecutiveCount > 0)
                    {
                        if (_rows[nRowIndex].Count > nHorizBlockCount - 1)
                        {
                            if (nHorizConsecutiveCount == _rows[nRowIndex][nHorizBlockCount - 1])
                            {
                                if (nBlockIndex == nHorizBlockCount - 1)
                                {
                                    if (j == _size - 1 && Math.Abs(_mat[nRowIndex][j]) == 1)
                                    {
                                        nEndBlockIndex = j;
                                    }
                                    else
                                    {
                                        nEndBlockIndex = j - 1;
                                    }
                                }
                            }
                        }
                        nHorizConsecutiveCount = 0;
                    }
                }

                if (nBlockIndex == nHorizBlockCount - 1 && nHorizConsecutiveCount == 1)
                {
                    nStartBlockIndex = j;
                }
            }
        }

        public void Serialize()
        {
            using (StreamWriter file = new StreamWriter(@"d:\Sources\EvoGenAlg\Dev\EvoGenAlg\EvoGenAlg\bin\Debug\Evolved.txt", true))
            {
                file.WriteLine("*************************************************************************************");
                foreach (List<int> row in _mat)
                {
                    string s = "";
                    foreach (int cell in row)
                    {
                        s += Math.Abs(cell).ToString();
                    }
                    file.WriteLine(s);
                }
                file.WriteLine("*************************************************************************************");
            }
        }

        public string GetRowAsString(int nRow)
        {
            string s = "";

            foreach (int cell in _mat[nRow])
            {
                s += Math.Abs(cell).ToString();
            }

            return s;
        }

        public string GetColAsString(int nCol)
        {
            string s = "";

            for(int i = 0; i< _size;i++)
            {
                s += Math.Abs(_mat[i][nCol]).ToString();
            }

            return s;
        }

        public static void SerializeList(int nIndex, List<string> validCombinations, bool bHorizontal)
        {
            string strFileName = @"d:\Sources\EvoGenAlg\Dev\EvoGenAlg\EvoGenAlg\bin\Debug\";
            if (bHorizontal)
            {
                strFileName += "HorizontalVariations_";
            }
            else
            {
                strFileName += "VerticalVariations_";
            }
            strFileName += nIndex.ToString() + ".txt";

            using (StreamWriter file = new StreamWriter(strFileName, false))
            {
                foreach (string s in validCombinations)
                {
                    file.WriteLine(s);
                }
            }
        }

        public void SerializeRow(int nRow)
        {
            string strFileName = @"d:\Sources\EvoGenAlg\Dev\EvoGenAlg\EvoGenAlg\bin\Debug\";
            strFileName += "HorizontalVariations_";
            strFileName += nRow.ToString() + ".txt";

            using (StreamWriter file = new StreamWriter(strFileName, true))
            {
                string s = "";
                foreach (int cell in _mat[nRow])
                {
                    s += Math.Abs(cell).ToString();
                }
                file.WriteLine(s);

            }
        }

        public void InitializeVariations()
        {
            for(int i = 0; i<_size;i++)
            {
                _HorizontalVariations.Add(DeserializeList(i, true));
                _VerticalVariations.Add(DeserializeList(i, false));
            }
        }

        public static List<string> DeserializeList(int nIndex, bool bHorizontal)
        {
            string strFileName = @"d:\Sources\EvoGenAlg\Dev\EvoGenAlg\EvoGenAlg\bin\Debug\";
            if (bHorizontal)
            {
                strFileName += "HorizontalVariations_";
            }
            else
            {
                strFileName += "VerticalVariations_";
            }
            strFileName += nIndex.ToString() + ".txt";

            List<string> list = new List<string>();
            using (StreamReader file = new StreamReader(strFileName))
            {

                string s = file.ReadLine();
                while(!string.IsNullOrWhiteSpace(s))
                {
                    if (!list.Contains(s))
                    {
                        list.Add(s);
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                    s = file.ReadLine();
                }
            }

            return list;
        }
     
    }
}

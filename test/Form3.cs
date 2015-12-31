using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using COM.MeshStudio.Lib.BasicComponent;

namespace test
{
    public partial class Form3 : Form
    {
        private string bcpath = @"C:\Users\David\AppData\Roaming\Ethereum\chaindata";
        private List<string> bcfiles = new List<string>();
        private string efile;
        private int blocklength = 100;
        Random r = new Random();
        private string zzzpath = @"e:\test.zzz";
        private string aaapath = @"e:\test.png";
        private Dictionary<int, byte[]> dict = new Dictionary<int, byte[]>();
        private int bsc = 16;

        public Form3()
        {
            InitializeComponent();
            textBox2.Text = blocklength.ToString();
            DirectoryInfo folder = new DirectoryInfo(bcpath);
            foreach (FileInfo file in folder.GetFiles())
            {
                bcfiles.Add(file.Name);
            }
            for (int i = 0; i < bsc; i++)
            {
                FileStream fs = new FileStream(bcpath + "\\" + bcfiles[i], FileMode.Open);
                byte[] bcc = new byte[fs.Length];
                fs.Read(bcc, 0, (int)fs.Length);
                fs.Close();
                dict.Add(i, bcc);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            efile = openFileDialog1.FileName;
            textBox1.Text = efile;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(textBox1.Text, FileMode.Open);
            byte[] tar = new byte[fs.Length];
            fs.Read(tar, 0, (int)fs.Length);
            fs.Close();
            int blen = Convert.ToInt32(textBox2.Text);
            List<byte> res = findCombinition(tar);
            FileAdaptor.WriteFileNoEnc(zzzpath, res.ToArray());
        }

        private List<byte> findCombinition(byte[] tar)
        {
            List<byte> result = new List<byte>();
            List<int> mList = new List<int>();
            List<int[]> mDict = new List<int[]>();
            List<byte[]> fileBlocks = new List<byte[]>();
            int index = 0;
            string resstring = "";
            while (index < tar.Length)
            {
                Dictionary<int, int[]> map = new Dictionary<int, int[]>();
                byte[] tar1 = new byte[tar.Length - index];
                for (int i = index; i < tar.Length; i++)
                {
                    tar1[i - index] = tar[i];
                }
                int m = 0;
                int[] fmcs = findBody(tar1);
                m = Math.Max(m, fmcs[0]);
                mList.Add(m);
                mDict.Add(fmcs);
                byte[] block = new byte[m];
                for(int i=0;i< m;i++)
                {
                    block[i] = tar1[i];
                }
                fileBlocks.Add(block);
                resstring += m + ",";
                index += m;
                textBox3.Text = resstring;
                label2.Text = index.ToString();
                Application.DoEvents();
            }
            int badcount = 0;
            int badstart = 0;
            int badlen = 0;
            for (int i = 0; i < mList.Count; i++)
            {
                int cm = mList[i];
                if (cm < 3)
                {
                    if (badcount == 0) badstart = i;
                    if (i == mList.Count - 1)
                    {
                        badlen += cm;
                        if (badlen < 128)
                        {
                            result.Add(Convert.ToByte(128));
                            result.Add(Convert.ToByte(badlen));
                        }
                        else if (badlen < 128 * 256)
                        {
                            result.Add(Convert.ToByte(128 + badlen / 256));
                            result.Add(Convert.ToByte(badlen % 256));
                        }
                        for (int j = 0; j < badcount; j++)
                        {
                            result.AddRange(fileBlocks[badstart + j]);
                        }
                        result.AddRange(fileBlocks[i]);
                    }
                    else
                    {
                        badcount ++;
                        badlen += cm;
                    }
                }
                else
                {
                    if (badcount >= 64)
                    {
                        if (badlen < 128)
                        {
                            result.Add(Convert.ToByte(128));
                            result.Add(Convert.ToByte(badlen));
                        }
                        else if (badlen < 128 * 256)
                        {
                            result.Add(Convert.ToByte(128 + badlen / 256));
                            result.Add(Convert.ToByte(badlen % 256));
                        }
                        for (int j = 0; j < badcount; j++)
                        {
                            result.AddRange(fileBlocks[badstart + j]);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < badcount; j++)
                        {
                            int[] fmcs = mDict[badstart + j];
                            int len = fmcs[0];
                            int bi = fmcs[1];
                            int ad = fmcs[2];
                            result.Add(Convert.ToByte(len * 16 + bi));
                            result.Add(Convert.ToByte(ad / 256));
                            result.Add(Convert.ToByte(ad % 256));
                        }
                    }
                    int[] fmcs1 = mDict[i];
                    int len1 = fmcs1[0];
                    int bi1 = fmcs1[1];
                    int ad1 = fmcs1[2];
                    result.Add(Convert.ToByte(len1 * 16 + bi1));
                    result.Add(Convert.ToByte(ad1 / 256));
                    result.Add(Convert.ToByte(ad1 % 256));
                    badcount = 0;
                    badlen = 0;
                }
            }
            return result;
        }

        private int[] findMaxCommon(byte[] tar, byte[] bcc)
        {
            Dictionary<int, int> resmap = new Dictionary<int, int>();
            int result = 0;
            for (int i = 1; i <= Math.Min(tar.Length, 15); i++)
            {
                byte[] tari = new byte[i];
                for (int j = 0; j < i; j++)
                {
                    tari[j] = tar[j];
                }
                for (int j = 0; j < bcc.Length - i; j += 64)
                {
                    byte[] bccj = new byte[i];
                    for (int k = j; k < j + i; k++)
                    {
                        bccj[k - j] = bcc[k];
                    }
                    for (int k = 0; k < i; k++)
                    {
                        if (tari[k] == bccj[k] && k == i - 1)
                        {
                            result = k + 1;
                            if (!resmap.ContainsKey(result))
                            {
                                resmap.Add(result, j / 64);
                            }
                        }
                        else if (tari[k] != bccj[k])
                        {
                            result = Math.Max(result, k);
                            if (!resmap.ContainsKey(result))
                            {
                                resmap.Add(result, j / 64);
                            }
                            break;
                        }
                    }
                }
                if (result < i) break;
                //label1.Text = i.ToString();
                //Application.DoEvents();
            }
            return new int[] { result, resmap.ContainsKey(result) ? resmap[result] : 0 };
        }

        private int[] findLigand(byte[] tar, byte[] bcc)
        {
            Dictionary<int, List<int>> resmap = new Dictionary<int, List<int>>();
            int result = 0;
            for (int i = 2; i <= Math.Min(tar.Length, 16); i+= 2)
            {
                byte[] tari = new byte[i];
                for (int j = 0; j < i; j++)
                {
                    tari[j] = tar[j];
                }
                int jlen = 256 * 32;
                for (int j = 0; j < Math.Min(32 * 256 * 256, bcc.Length - jlen); j += jlen)
                {
                    byte[] bccj = new byte[jlen];
                    for (int k = j; k < j + jlen; k++)
                    {
                        bccj[k - j] = bcc[k];
                    }
                    List<int> offsets = new List<int>();
                    int totaloffset = 0;
                    int len = 0;
                    for (int k = 0; k < i; k+= 2)
                    {
                        bool doBreak = true;
                        //if (k == 0 && tari[k] != bccj[totaloffset + k]) break;
                        for (int i2 = 0; i2 < 64; i2++)
                        {
                            if (tari[k] == bccj[totaloffset + k + i2] && tari[k + 1] == bccj[totaloffset + k + i2 + 1] || tari[k] == bccj[totaloffset + k + i2] && tari[k + 1] == bccj[totaloffset + k + i2 + 2])
                            {
                                len = k + 2;
                                offsets.Add(i2);
                                totaloffset += i2;
                                doBreak = false;
                                break;
                            }
                        }
                        if (doBreak) break;
                    }
                    result = Math.Max(result, len);
                    if (!resmap.ContainsKey(result))
                    {
                        offsets.Insert(0, j / jlen);
                        resmap.Add(result, offsets);
                    }
                }
                if (result < i) break;
                label1.Text = i.ToString();
                Application.DoEvents();
            }
            List<int> res = new List<int>();
            res.Add(result);
            if(resmap.ContainsKey(result))
            {
                res.AddRange(resmap[result]);
            }
            return res.ToArray();
        }

        private int[] findBody(byte[] tar)
        {
            Dictionary<int, List<int>> resmap = new Dictionary<int, List<int>>();
            int result = 0;
            for (int i = 1; i <= Math.Min(tar.Length, 7); i++)
            {
                List<int> para = new List<int>();
                bool found = false;
                byte[] tari = new byte[i];
                for (int j = 0; j < i; j++)
                {
                    tari[j] = tar[j];
                }
                for (int j = 0; j < bsc; j++)
                {
                    byte[] bcc = dict[j];
                    for (int k = 0; k < Math.Min(32 * 256 * 256, bcc.Length - i); k += 32)
                    {
                        byte[] bccj = new byte[i];
                        for (int l = 0; l < i; l++)
                        {
                            bccj[l] = bcc[k + l];
                        }

                        if (arrayEqual(tari, bccj))
                        //if (Encoding.UTF8.GetString(tari) == Encoding.UTF8.GetString(bccj))
                        {
                            result = Math.Max(result, i);
                            if (!resmap.ContainsKey(result))
                            {
                                para.Add(i);
                                para.Add(j);
                                para.Add(k / 32);
                                resmap.Add(result, para);
                            }
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
                if (result < i) break;
                label1.Text = i.ToString();
                Application.DoEvents();
            }
            return resmap[result].ToArray();
        }

        private bool arrayEqual(byte[] a1, byte[] a2)
        {
            for(int i=0;i<a1.Length;i++)
            {
                if (a1[i] != a2[i]) return false;
            }
            return true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<byte> aaa = new List<byte>();
            byte[] res = FileAdaptor.ReadFileNoEnc(zzzpath);
            //string resstring = "";
            for (int i = 0; i < res.Length; i ++)
            {
                byte r1 = res[i];
                if (r1 >= 128)
                {
                    byte r2 = res[i + 1];
                    int len = (r1 - 128) * 128 + r2;
                    for(int j=0;j< len;j++)
                    {
                        aaa.Add(res[i + 2 + j]);
                    }
                    i += 2 + len - 1;
                }
                else
                {
                    byte r2 = res[i + 1];
                    byte r3 = res[i + 2];
                    int len = r1 / 16;
                    int bi = r1 % 16;
                    int ad = r2 * 256 + r3;
                    
                    List<byte> piece = new List<byte>();
                    for (int j = 0; j < len; j++)
                    {
                        byte[] bcc = dict[bi];
                        piece.Add(bcc[ad * 32 + j]);
                    }
                    aaa.AddRange(piece);
                    i += 2;
                }
            }
            FileStream fsr = new FileStream(aaapath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fsr);
            bw.Write(aaa.ToArray());
            bw.Close();
            fsr.Close();
            //textBox4.Text = resstring;
        }
    }
}

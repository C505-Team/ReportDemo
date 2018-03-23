using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;



namespace ReportDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public bool fig = false;
        //加载窗体，并显示左边目录视图（树形TreeView）
        private void Form1_Load(object sender, EventArgs e)
        {
            #region 创建一个树形目录
            TreeNode node;
            this.treeView1.Nodes[0].Tag = "详单";
            string path = @"E:\现场勘验\case20180320060917\采集报告\DESKTOP-FRJ5C8B_20180320_061335\MainHtmls\";
            DirectoryInfo dir = new DirectoryInfo(path);
            string[] s = Directory.GetLogicalDrives(); //获取计算机上的逻辑驱动器
            foreach (DirectoryInfo di in dir.GetDirectories()) //显示目录视图
            {
                string str=di.FullName;
                treeView1.ImageList.Images.Add(MyFile.GetFileIcon(str, true));
                node = new TreeNode(di.Name, treeView1.ImageList.Images.Count - 1, treeView1.ImageList.Images.Count - 1);
                node.Tag = di.FullName;
                treeView1.Nodes[0].Nodes.Add(node);
                try
                {
                    string[] s2 = Directory.GetDirectories(node.Tag.ToString());
                    if (s2.Length > 0)
                        node.Nodes.Add("-1");

                }
                catch { }

            }
            #endregion

        }
        


        //响应点击TreeView事件,生成下一级TreeView视图（目录扩展前）
        #region 生成下一级TreeView视图
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {


            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "-1")
            {
                e.Node.Nodes.Clear();
                //获取选择地址下的所有文件列表
                string[] s1 = Directory.GetDirectories(e.Node.Tag.ToString());
                foreach (string str in s1)
                {
                    string shortfilename = Path.GetFileName(str);//获取其中的一个文件名
                    TreeNode node = new TreeNode(shortfilename);
                    node.Tag = str;
                    e.Node.Nodes.Add(node);
                }

            }

        } 
        #endregion

        //根据地址显示当前路径下的所有文件（显示在右边的listview中）
        private void ShowFiles(string path)
        {
            ListViewItem li;
            cmbpath.Items.Insert(0, path);
            cmbpath.Text = path;
            //this.Text = path;
            listView1.Items.Clear();
            listView1.LargeImageList.Images.Clear();
            listView1.SmallImageList.Images.Clear();
            if (path == "我的电脑")
            {
                try
                {
                    string[] s = Directory.GetLogicalDrives();
                    foreach (string str in s)
                    {
                        listView1.SmallImageList.Images.Add(MyFile.GetFileIcon(str, false));
                        listView1.LargeImageList.Images.Add(MyFile.GetFileIcon(str, false));
                        li = new ListViewItem(str, listView1.LargeImageList.Images.Count - 1);
                        li.Tag = str;
                        li.SubItems.Add("");
                        li.SubItems.Add(Directory.GetLastWriteTime(str).ToLongDateString());
                        li.SubItems.Add(MyFile.GetFileType(str));
                        listView1.Items.Add(li);
                    }

                }
                catch
                {

                }

            }
            else
            {
                try
                {
                    fileSystemWatcher1.Path = path;
                    String[] s = Directory.GetDirectories(path);
                    //richTextBox1.Text = s.Length + '\n' + s[0] + '\n' + s[1];
                    foreach (string str in s)
                    {
                        listView1.LargeImageList.Images.Add(MyFile.GetFileIcon(str, true));
                        listView1.SmallImageList.Images.Add(MyFile.GetFileIcon(str, false));

                        li = new ListViewItem(Path.GetFileName(str), listView1.SmallImageList.Images.Count - 1);
                        li.Tag = str;
                        li.SubItems.Add("");
                        li.SubItems.Add("文件夹");
                        li.SubItems.Add(Directory.GetCreationTime(str).ToLongDateString());
                        this.listView1.Items.Add(li);
                    }
                    String[] ss = Directory.GetFiles(path);
                    foreach (string str in ss)
                    {
                        listView1.LargeImageList.Images.Add(MyFile.GetFileIcon(str, true));
                        listView1.SmallImageList.Images.Add(MyFile.GetFileIcon(str, false));
                        li = new ListViewItem(Path.GetFileName(str), listView1.SmallImageList.Images.Count - 1);
                        li.Tag = str;
                        FileInfo fi = new FileInfo(str);

                        li.SubItems.Add((fi.Length / 1024).ToString("#,##0") + "KB");
                        li.SubItems.Add(MyFile.GetFileType(str));
                        li.SubItems.Add(File.GetCreationTime(str).ToLongDateString());

                        this.listView1.Items.Add(li);
                    }
                }
                catch
                { }
            }



        }

        //目录扩展后
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                ShowFiles(e.Node.Tag.ToString());
            }
            catch
            {

            }

        }
        //菜单中查看方式，根据地址旁边输入的地址显示文件
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            try
            {
                ShowFiles(cmbpath.Text);
            }
            catch
            { }
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (listView1.SelectedItems[0] != null)
            {
                if (listView1.SelectedItems[0].Text == "文件夹" || listView1.SelectedItems[0].Text.Length == 3)
                    ShowFiles(listView1.SelectedItems[0].Tag.ToString());
                else
                    Process.Start(listView1.SelectedItems[0].Tag.ToString());
            }

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = !splitContainer1.Panel1Collapsed;


        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (cmbpath.Text != "我的电脑")
            {
                try
                {
                    ShowFiles(Directory.GetParent(cmbpath.Text).FullName);
                }
                catch
                {
                    ShowFiles("我的电脑");
                }
            }

        }
        //后退
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (cmbpath.Text != "我的电脑")
            {
                try
                {
                    ShowFiles(Directory.GetParent(cmbpath.Text).FullName);
                }
                catch
                {
                    ShowFiles("我的电脑");
                }
            }

        }
        //菜单中文件打开
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem li in listView1.SelectedItems)
            {
                try
                {
                    if (li.SubItems[2].Text == "文件夹" || li.SubItems[0].Text.Length == 3)
                        ShowFiles(listView1.SelectedItems[0].Tag.ToString());
                    else
                        Process.Start(li.Tag.ToString());
                }
                catch
                {

                }
            }

        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] filelist = new string[listView1.SelectedItems.Count];
            //将被选中项中保存的文件名称保存到数组中
            for (int i = 0; i < listView1.SelectedItems.Count; i++)
            {
                filelist[i] = listView1.SelectedItems[i].Tag.ToString();
            }
            //将数据（文件的名称）置于系统剪贴板中，完成数据的复制操作
            Clipboard.SetData(DataFormats.FileDrop, filelist);

        }

        //文件复制
        private void CopyDir(string source, string dest)
        {
            string CurrentPath;
            string[] Files;
            string[] Dirs;
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);

            }
            CurrentPath = dest + "\\";
            Files = Directory.GetFiles(source);
            Dirs = Directory.GetDirectories(source);

            foreach (string s in Files)
            {
                if (File.Exists(CurrentPath + Path.GetFileName(s)))
                {
                    if (MessageBox.Show("文件" + CurrentPath + Path.GetFileName(s) + "已经存在，要覆盖现有文件吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        File.Copy(s, CurrentPath + Path.GetFileName(s), true);

                }
                else
                    File.Copy(s, CurrentPath + Path.GetFileName(s));


            }
            //复制原目录下的文件夹
            foreach (string s in Dirs)
            {
                CopyDir(s, CurrentPath + Path.GetFileName(s));
            }

        }
        //快快捷菜单中文件粘贴
        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        { //IDataObject Data = Clipboard.GetDataObject();//获取剪切板上的数据
            string[] files = (string[])Clipboard.GetData(DataFormats.FileDrop);//获取剪贴板上保存的文件名数组

            string newfilepath = cmbpath.Text;            //取当前目录
            if (newfilepath.Length > 3)
                newfilepath = newfilepath + "\\";

            for (int i = 0; i < files.Length; i++)
            {
                if (MyFile.GetFileType(files[i]) != "文件夹")
                {
                    if (File.Exists(newfilepath + Path.GetFileName(files[i])))//如果文件已经存在
                    {
                        if (MessageBox.Show("文件" + newfilepath + Path.GetFileName(files[i]) + "已经存在,要覆盖现有文件吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            File.Copy(files[i], newfilepath + Path.GetFileName(files[i]), true);//文件复制
                            continue;
                        }
                    }
                    try
                    {
                        File.Copy(files[i], newfilepath + Path.GetFileName(files[i]), true);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);

                    }

                }
                else
                {
                    try
                    {
                        CopyDir(files[i], newfilepath + Path.GetFileName(files[i]));   //复制整个目录
                    }

                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                    }




                }


            }


        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("你确认要删除所选文件吗？","警告",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes)

                foreach(ListViewItem li in listView1.SelectedItems)
                    try
                    {
                        if (li.SubItems[2].Text == "文件夹")//如果是文件
                            Directory.Delete(li.Tag.ToString(), true);
                        else if (li.SubItems[0].Text.Length != 3)//如果不是驱动器
                        {
                            File.Delete(li.Tag.ToString());
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
            
                


        }

        private void fileSystemWatcher1_Deleted(object sender, FileSystemEventArgs e)
        {
            foreach (ListViewItem li in listView1.SelectedItems)
            {
                if (li.Tag.ToString() == e.FullPath)
                    li.Remove();
            }
        }

      
    }
}

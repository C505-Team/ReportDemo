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
using ExtendedRichTextBox;



namespace ReportDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            currentFile = "";
        }

        #region "Declaration"

        private string currentFile;
        private int checkPrint;

        #endregion

        public bool fig = false;
        //加载窗体，并显示左边目录视图（树形TreeView）
        private void Form1_Load(object sender, EventArgs e)
        {
            
            #region 创建一个树形目录
            TreeNode node;
            this.treeView1.Nodes[0].Tag = "详单";
            string path = @"C:";
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


        //文档报告的操作
        #region "Menu Methods"


        private void NewToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (rtbDoc.Modified == true)
                {
                    System.Windows.Forms.DialogResult answer;
                    answer = MessageBox.Show("Save current document before creating new document?", "Unsaved Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (answer == System.Windows.Forms.DialogResult.No)
                    {
                        currentFile = "";
                        this.Text = "Editor: New Document";
                        rtbDoc.Modified = false;
                        rtbDoc.Clear();
                        return;
                    }
                    else
                    {
                        SaveToolStripMenuItem_Click(this, new EventArgs());
                        rtbDoc.Modified = false;
                        rtbDoc.Clear();
                        currentFile = "";
                        this.Text = "Editor: New Document";
                        return;
                    }
                }
                else
                {
                    currentFile = "";
                    this.Text = "Editor: New Document";
                    rtbDoc.Modified = false;
                    rtbDoc.Clear();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void OpenToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (rtbDoc.Modified == true)
                {
                    System.Windows.Forms.DialogResult answer;
                    answer = MessageBox.Show("Save current file before opening another document?", "Unsaved Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (answer == System.Windows.Forms.DialogResult.No)
                    {
                        rtbDoc.Modified = false;
                        OpenFile();
                    }
                    else
                    {
                        SaveToolStripMenuItem_Click(this, new EventArgs());
                        OpenFile();
                    }
                }
                else
                {
                    OpenFile();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void OpenFile()
        {
            try
            {
                OpenFileDialog1.Title = "RTE - Open File";
                OpenFileDialog1.DefaultExt = "rtf";
                OpenFileDialog1.Filter = "Rich Text Files|*.rtf|Text Files|*.txt|HTML Files|*.htm|All Files|*.*";
                OpenFileDialog1.FilterIndex = 1;
                OpenFileDialog1.FileName = string.Empty;

                if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
                {

                    if (OpenFileDialog1.FileName == "")
                    {
                        return;
                    }

                    string strExt;
                    strExt = System.IO.Path.GetExtension(OpenFileDialog1.FileName);
                    strExt = strExt.ToUpper();

                    if (strExt == ".RTF")
                    {
                        rtbDoc.LoadFile(OpenFileDialog1.FileName, RichTextBoxStreamType.RichText);
                    }
                    else
                    {
                        System.IO.StreamReader txtReader;
                        txtReader = new System.IO.StreamReader(OpenFileDialog1.FileName);
                        rtbDoc.Text = txtReader.ReadToEnd();
                        txtReader.Close();
                        txtReader = null;
                        rtbDoc.SelectionStart = 0;
                        rtbDoc.SelectionLength = 0;
                    }

                    currentFile = OpenFileDialog1.FileName;
                    rtbDoc.Modified = false;
                    this.Text = "Editor: " + currentFile.ToString();
                }
                else
                {
                    MessageBox.Show("Open File request cancelled by user.", "Cancelled");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void SaveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (currentFile == string.Empty)
                {
                    SaveAsToolStripMenuItem_Click(this, e);
                    return;
                }

                try
                {
                    string strExt;
                    strExt = System.IO.Path.GetExtension(currentFile);
                    strExt = strExt.ToUpper();
                    if (strExt == ".RTF")
                    {
                        rtbDoc.SaveFile(currentFile);
                    }
                    else
                    {
                        System.IO.StreamWriter txtWriter;
                        txtWriter = new System.IO.StreamWriter(currentFile);
                        txtWriter.Write(rtbDoc.Text);
                        txtWriter.Close();
                        txtWriter = null;
                        rtbDoc.SelectionStart = 0;
                        rtbDoc.SelectionLength = 0;
                    }

                    this.Text = "Editor: " + currentFile.ToString();
                    rtbDoc.Modified = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "File Save Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }


        }


        private void SaveAsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

            try
            {
                SaveFileDialog1.Title = "RTE - Save File";
                SaveFileDialog1.DefaultExt = "rtf";
                SaveFileDialog1.Filter = "Rich Text Files|*.rtf|Text Files|*.txt|HTML Files|*.htm|All Files|*.*";
                SaveFileDialog1.FilterIndex = 1;

                if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                {

                    if (SaveFileDialog1.FileName == "")
                    {
                        return;
                    }

                    string strExt;
                    strExt = System.IO.Path.GetExtension(SaveFileDialog1.FileName);
                    strExt = strExt.ToUpper();

                    if (strExt == ".RTF")
                    {
                        rtbDoc.SaveFile(SaveFileDialog1.FileName, RichTextBoxStreamType.RichText);
                    }
                    else
                    {
                        System.IO.StreamWriter txtWriter;
                        txtWriter = new System.IO.StreamWriter(SaveFileDialog1.FileName);
                        txtWriter.Write(rtbDoc.Text);
                        txtWriter.Close();
                        txtWriter = null;
                        rtbDoc.SelectionStart = 0;
                        rtbDoc.SelectionLength = 0;
                    }

                    currentFile = SaveFileDialog1.FileName;
                    rtbDoc.Modified = false;
                    this.Text = "Editor: " + currentFile.ToString();
                    MessageBox.Show(currentFile.ToString() + " saved.", "File Save");
                }
                else
                {
                    MessageBox.Show("Save File request cancelled by user.", "Cancelled");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void ExitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (rtbDoc.Modified == true)
                {
                    System.Windows.Forms.DialogResult answer;
                    answer = MessageBox.Show("Save this document before closing?", "Unsaved Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (answer == System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }
                    else
                    {
                        rtbDoc.Modified = false;
                        Application.Exit();
                    }
                }
                else
                {
                    rtbDoc.Modified = false;
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void SelectAllToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                rtbDoc.SelectAll();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to select all document content.", "RTE - Select", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void CopyToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                rtbDoc.Copy();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to copy document content.", "RTE - Copy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void CutToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                rtbDoc.Cut();
            }
            catch
            {
                MessageBox.Show("Unable to cut document content.", "RTE - Cut", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void PasteToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                rtbDoc.Paste();
            }
            catch
            {
                MessageBox.Show("Unable to copy clipboard content to document.", "RTE - Paste", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void SelectFontToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (!(rtbDoc.SelectionFont == null))
                {
                    FontDialog1.Font = rtbDoc.SelectionFont;
                }
                else
                {
                    FontDialog1.Font = null;
                }
                FontDialog1.ShowApply = true;
                if (FontDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    rtbDoc.SelectionFont = FontDialog1.Font;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void FontColorToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                ColorDialog1.Color = rtbDoc.ForeColor;
                if (ColorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    rtbDoc.SelectionColor = ColorDialog1.Color;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void BoldToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

        }




        private void ItalicToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

        }





        private void UnderlineToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

        }





        private void NormalToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (!(rtbDoc.SelectionFont == null))
                {
                    System.Drawing.Font currentFont = rtbDoc.SelectionFont;
                    System.Drawing.FontStyle newFontStyle;
                    newFontStyle = FontStyle.Regular;
                    rtbDoc.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void PageColorToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                ColorDialog1.Color = rtbDoc.BackColor;
                if (ColorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    rtbDoc.BackColor = ColorDialog1.Color;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void mnuUndo_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (rtbDoc.CanUndo)
                {
                    rtbDoc.Undo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void mnuRedo_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (rtbDoc.CanRedo)
                {
                    rtbDoc.Redo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void LeftToolStripMenuItem_Click_1(object sender, System.EventArgs e)
        {
            try
            {
                rtbDoc.SelectionAlignment = HorizontalAlignment.Left;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void CenterToolStripMenuItem_Click_1(object sender, System.EventArgs e)
        {

        }




        private void RightToolStripMenuItem_Click_1(object sender, System.EventArgs e)
        {

        }




        private void AddBulletsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                rtbDoc.BulletIndent = 10;
                rtbDoc.SelectionBullet = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void RemoveBulletsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                rtbDoc.SelectionBullet = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void mnuIndent0_Click(object sender, System.EventArgs e)
        {
            try
            {
                rtbDoc.SelectionIndent = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void mnuIndent5_Click(object sender, System.EventArgs e)
        {
            try
            {
                rtbDoc.SelectionIndent = 5;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void mnuIndent10_Click(object sender, System.EventArgs e)
        {
            try
            {
                rtbDoc.SelectionIndent = 10;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void mnuIndent15_Click(object sender, System.EventArgs e)
        {
            try
            {
                rtbDoc.SelectionIndent = 15;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void mnuIndent20_Click(object sender, System.EventArgs e)
        {
            try
            {
                rtbDoc.SelectionIndent = 20;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }

        }




        private void FindToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                frmFind f = new frmFind(this);
                f.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void FindAndReplaceToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                frmReplace f = new frmReplace(this);
                f.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void PreviewToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

        }




        private void PrintToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

        }




        private void mnuPageSetup_Click(object sender, System.EventArgs e)
        {
            try
            {
                PageSetupDialog1.Document = PrintDocument1;
                PageSetupDialog1.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void InsertImageToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

            OpenFileDialog1.Title = "RTE - Insert Image File";
            OpenFileDialog1.DefaultExt = "rtf";
            OpenFileDialog1.Filter = "Bitmap Files|*.bmp|JPEG Files|*.jpg|GIF Files|*.gif";
            OpenFileDialog1.FilterIndex = 1;
            OpenFileDialog1.ShowDialog();

            if (OpenFileDialog1.FileName == "")
            {
                return;
            }

            try
            {
                string strImagePath = OpenFileDialog1.FileName;
                Image img;
                img = Image.FromFile(strImagePath);
                Clipboard.SetDataObject(img);
                DataFormats.Format df;
                df = DataFormats.GetFormat(DataFormats.Bitmap);
                if (this.rtbDoc.CanPaste(df))
                {
                    this.rtbDoc.Paste(df);
                }
            }
            catch
            {
                MessageBox.Show("Unable to insert image format selected.", "RTE - Paste", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void rtbDoc_SelectionChanged(object sender, EventArgs e)
        {
            tbrBold.Checked = rtbDoc.SelectionFont.Bold;
            tbrItalic.Checked = rtbDoc.SelectionFont.Italic;
            tbrUnderline.Checked = rtbDoc.SelectionFont.Underline;
        }




        #endregion




        #region Toolbar Methods


        private void tbrSave_Click(object sender, System.EventArgs e)
        {
            SaveToolStripMenuItem_Click(this, e);
        }


        private void tbrOpen_Click(object sender, System.EventArgs e)
        {
            OpenToolStripMenuItem_Click(this, e);
        }


        private void tbrNew_Click(object sender, System.EventArgs e)
        {
            NewToolStripMenuItem_Click(this, e);
        }


        private void tbrBold_Click(object sender, System.EventArgs e)
        {
            BoldToolStripMenuItem_Click(this, e);
        }


        private void tbrItalic_Click(object sender, System.EventArgs e)
        {
            ItalicToolStripMenuItem_Click(this, e);
        }


        private void tbrUnderline_Click(object sender, System.EventArgs e)
        {
            UnderlineToolStripMenuItem_Click(this, e);
        }


        private void tbrFont_Click(object sender, System.EventArgs e)
        {
            SelectFontToolStripMenuItem_Click(this, e);
        }


        private void tbrLeft_Click(object sender, System.EventArgs e)
        {
            rtbDoc.SelectionAlignment = HorizontalAlignment.Left;
        }


        private void tbrCenter_Click(object sender, System.EventArgs e)
        {
            rtbDoc.SelectionAlignment = HorizontalAlignment.Center;
        }


        private void tbrRight_Click(object sender, System.EventArgs e)
        {
            rtbDoc.SelectionAlignment = HorizontalAlignment.Right;
        }


        private void tbrFind_Click(object sender, System.EventArgs e)
        {
            frmFind f = new frmFind(this);
            f.Show();
        }


        private void tspColor_Click(object sender, EventArgs e)
        {
            FontColorToolStripMenuItem_Click(this, new EventArgs());
        }




        #endregion




        #region Printing


        private void PrintDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

            checkPrint = 0;

        }



        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

        }





        #endregion




        #region Form Closing Handler


        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (rtbDoc.Modified == true)
                {
                    System.Windows.Forms.DialogResult answer;
                    answer = MessageBox.Show("Save current document before exiting?", "Unsaved Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (answer == System.Windows.Forms.DialogResult.No)
                    {
                        rtbDoc.Modified = false;
                        rtbDoc.Clear();
                        return;
                    }
                    else
                    {
                        SaveToolStripMenuItem_Click(this, new EventArgs());
                    }
                }
                else
                {
                    rtbDoc.Clear();
                }
                currentFile = "";
                this.Text = "Editor: New Document";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }


        #endregion
    }
}

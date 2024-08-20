using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;


namespace IPOS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Collect all the data here, because we have to display it on the datagrid with uniq indexes.
        private ObservableCollection<object> displayedData = new ObservableCollection<object>();
        //That's where we collect the data, that'll be processed trought project generation.
        private List<rawData> dataColl = new List<rawData>();
        //We hold data for the map as well...
        private List<Cartesian> mapCoords = new List<Cartesian>();
        //We have this lists, it's a little ridiculous 'cause we are only using for count.. 
        private List<string> objsName = new List<string>();
        private List<string> lodsName = new List<string>();
        //Title
        private string title = "IPOS v0.1 BETA - Set up project ";
        private string currLod;
        private int selectedItemIndex = -1;
        private int lodCount = 0;
        private bool HasFastMan = false;
        //That will passed as constructor parameter when we, create the map dialog.. 
        private int Altered_WorldSize = 6000;
        private int Altered_DrawDis = 300;

        //It's just a red brush color, that is never used...
        Brush red;

        public MainWindow()
        {
            InitializeComponent();
            //Read all the settings from the beginings
            readINIsettings();
        }

        private void setUI()
        {
            this.Title = title;
        }

        private void readINIsettings()
        {
            try
            {
                if (File.Exists("IPOS.ini"))
                {
                    using (StreamReader iniRD = new StreamReader("IPOS.ini"))
                    {
                        string line;

                        while ((line = iniRD.ReadLine()) != null)
                        {
                            if (line.Contains('='))
                            {
                                string[] dataSP = line.Split('=');

                                if (dataSP[0].Trim() == "GTA_SA_PATH")
                                {
                                    gta_sa_pth.Text = dataSP[1];
                                }
                                else if (dataSP[0].Trim() == "LAST_IPOS")
                                {
                                    loadIPOS(dataSP[1]);
                                    ShowIposData();
                                }
                                else if (dataSP[0].Trim() == "HasFastMan")
                                {
                                    bool fastman = bool.Parse(dataSP[1]);
                                    this.HasFastMan = fastman;
                                    fastman92.IsChecked = fastman;
                                    setFastMan();
                                }
                            }
                        }
                    }
                }
                else
                {
                    errShow("IPOS.ini file not exist!");
                }
            }
            catch (Exception ex)
            {
                errShow("An error occured while read from settings: " + ex);
            }
        }

        private void dragOn(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {

            }
        }

        // ------  Basic controls ------ //
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnHoverClo(object sender, RoutedEventArgs e)
        {
            close_img.Source = new BitmapImage(new Uri(@"/close_hover.png", UriKind.Relative));
        }

        private void LeaveHoverClo(object sender, RoutedEventArgs e)
        {
            close_img.Source = new BitmapImage(new Uri(@"/close.png", UriKind.Relative));
        }

        private void OnHoverMin(object sender, RoutedEventArgs e)
        {
            minimize_img.Source = new BitmapImage(new Uri(@"/minimize_hover.png", UriKind.Relative));
        }

        private void LeaveHoverMin(object sender, RoutedEventArgs e)
        {
            minimize_img.Source = new BitmapImage(new Uri(@"/minimize.png", UriKind.Relative));
        }

        private void OnHoverCre(object sender, System.Windows.Input.MouseEventArgs e)
        {
            create_img.Source = new BitmapImage(new Uri(@"/create_hover.png", UriKind.Relative));
        }

        private void LeaveHoverCre(object sender, System.Windows.Input.MouseEventArgs e)
        {
            create_img.Source = new BitmapImage(new Uri(@"/create.png", UriKind.Relative));
        }

        private void mapview_Click(object sender, RoutedEventArgs e)
        {
            showmap map = new showmap(mapCoords, Altered_WorldSize);
            map.ShowDialog();
        }

        private void fastman92_Check(object sender, RoutedEventArgs e)
        {
            setFastMan();
            writeINIsettings("HasFastMan", fastman92.IsChecked.ToString());
        }

        private void setFastMan()
        {
            if (fastman92.IsChecked == true)
            {
                if (string.IsNullOrEmpty(gta_sa_pth.Text) || !Directory.Exists(gta_sa_pth.Text))
                {
                    errShow("Please select your GTA SA root folder.");
                    fastman92.IsChecked = false;
                    return;
                }
                string[] files = Directory.GetFiles(gta_sa_pth.Text, "*fastman*.ini", SearchOption.TopDirectoryOnly);

                if (files.Length == 0)
                {
                    errShow("There is no fastman92 - limit adjuster (.ini) file.");
                    fastman92.IsChecked = false;
                    return;
                }

                fastman92Read(files[0]);
            }
            else
            {
                Altered_WorldSize = 6000;
                Altered_DrawDis = 300;
            }
        }

        private void blender_Click(object sender, RoutedEventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                folderBrowser.SelectedPath = @"C:\";

                if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string des = Path.Combine(folderBrowser.SelectedPath, "IPOS Import_Export.zip");

                    var assembly = Assembly.GetExecutingAssembly();
                    var pluginName = "IPOS.IPOS Import_Export.zip";

                    using (Stream pluginStream = assembly.GetManifestResourceStream(pluginName))
                    {
                        if (pluginStream == null)
                        {
                            errShow("Resource not found.");
                            return;
                        }

                        using (FileStream fileStream = new FileStream(des, FileMode.Create, FileAccess.Write))
                        {
                            pluginStream.CopyTo(fileStream);
                        }
                    }
                    successMessageShow("File successfully exported to " + des);
                }
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        //--------------------------------------------//

        private void fastman92Read(string pth)
        {
            //We iterate trought all the lines, we just want to get those to parameters 'cause that's relevant for our purpose.
            try
            {
                using (StreamReader fastmanRD = new StreamReader(pth))
                {
                    string line;
                    string[] findProps = { "World map size", "LOD distance" };

                    while ((line = fastmanRD.ReadLine()) != null)
                    {
                        if (!line.TrimStart().StartsWith("#") && line.Contains('='))
                        {
                            string[] dataSP = line.Split('=');

                            if (dataSP[0].Trim() == findProps[0])
                            {
                                Altered_WorldSize = int.Parse(dataSP[1]);
                            }
                            else if (dataSP[0].Trim() == findProps[1])
                            {
                                Altered_DrawDis = (int)double.Parse(dataSP[1]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errShow("Error occurred while parsing fastman92: " + ex.Message);
            }
        }

        private void writeINIsettings(string tar, string val)
        {
            if (File.Exists("IPOS.ini"))
            {
                //That's where we change the params by a target and val
                string[] lines = File.ReadAllLines("IPOS.ini");

                using (StreamWriter iniWR = new StreamWriter("IPOS.ini"))
                {
                    foreach (string line in lines)
                    {
                        if (line.Contains('='))
                        {
                            string[] dataSP = line.Split('=');
                            if (dataSP[0].Trim() == tar)
                            {
                                iniWR.WriteLine($"{tar} = {val}");
                            }
                            else
                            {
                                iniWR.WriteLine(line);
                            }
                        }
                        else
                        {
                            iniWR.WriteLine(line);
                        }
                    }
                }
            }
            else
            {
                errShow("IPOS.ini file not exist!");
            }
        }

        private void iposData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //There's where I used the displayData variable 'cause we have the ability to determine when the selection changed, by an LOD section or rawData for further ops.
            selectedItemIndex = iposData.SelectedIndex;
            if (selectedItemIndex >= 0)
            {
                var selectedItem = displayedData[selectedItemIndex];

                if (selectedItem is rawData rawItem)
                {
                    EditData editData = new EditData(rawItem, Altered_DrawDis);
                    if (editData.ShowDialog() == true)
                    {
                        changeIposData(rawItem, editData);
                    }
                    editData = null;
                }
                else if (selectedItem is LOD lodItem)
                {
                    EditLOD editLOD = new EditLOD(lodItem.drawDis);
                    if (editLOD.ShowDialog() == true)
                    {
                        changeIposLOD(lodItem, editLOD);
                    }
                    editLOD = null;
                }

                iposData.SelectedIndex = -1;
            }
        }

        private void changeIposData(rawData rawItem, EditData editData)
        {
            //We planly change all the rawdata here.
            if (editData.ApplToAll)
            {
                foreach (var item in dataColl)
                {
                    item.modelName = editData.UpdatedData.modelName;
                    item.interior = editData.UpdatedData.interior;
                    item.drawDis = editData.UpdatedData.drawDis;
                    item.flags = editData.UpdatedData.flags;
                    if (item.lodData != null)
                    {
                        item.lodData.interior = editData.UpdatedData.interior;
                        item.lodData.flags = editData.UpdatedData.flags;
                    }
                }
            }
            else
            {
                rawItem.modelName = editData.UpdatedData.modelName;
                rawItem.interior = editData.UpdatedData.interior;
                rawItem.drawDis = editData.UpdatedData.drawDis;
                rawItem.flags = editData.UpdatedData.flags;
                if (rawItem.lodData != null)
                {
                    rawItem.lodData.interior = editData.UpdatedData.interior;
                    rawItem.lodData.flags = editData.UpdatedData.flags;
                }
            }
            iposData.ItemsSource = null;
            ShowIposData();
        }

        private void changeIposLOD(LOD lodItem, EditLOD editLod)
        {
            //We planly change all the LOD here.
            if (editLod.ApplToAll)
            {
                foreach (var item in dataColl)
                {
                    if (item.lodData != null)
                    {
                        item.lodData.drawDis = editLod.DrawDis;
                    }
                }
            }
            else
            {
                rawData parentRawData = dataColl.FirstOrDefault(item => item.lodData == lodItem);
                if (parentRawData != null)
                {
                    parentRawData.lodData.drawDis = editLod.DrawDis;
                }
            }
            iposData.ItemsSource = null;
            ShowIposData();
        }

        //Open .ipos file
        private void openIPOS_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ipos_diag = new System.Windows.Forms.OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "Item Position File (*.ipos)|*.ipos",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (ipos_diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dataColl.Clear();
                objsName.Clear();
                lodCount = 0;

                this.Title = title + "- " + System.IO.Path.GetFileName(ipos_diag.FileName);
                loadIPOS(ipos_diag.FileName);
                ShowIposData();
                writeINIsettings("LAST_IPOS", ipos_diag.FileName); 
            }
        }

        private void loadIPOS(string pth)
        {
            //We check if the file even exist
            if (File.Exists(pth))
            {
                using (StreamReader iposRD = new StreamReader(pth))
                {
                    int nameIndex = 0;
                    string rawline = "";
                    string objname = "";
                    double[] Coor = null;
                    double[] Q = null;


                    while ((rawline = iposRD.ReadLine()) != null)
                    {
                        rawline = rawline.Trim();

                        if (rawline == "end")
                        {
                            //We load all the incoming data from the .ipos with this parser.
                            rawData rw = new rawData(objname, objname, null, Coor[0], Coor[1], Coor[2], Q[0], Q[1], Q[2], Q[3], false, 0, null, 300, 0);
                            dataColl.Add(rw);
                            //It's not so memory wise to duplicate those too, but i done that 'cause it's give us a more handy transfering between the map dialog..
                            Cartesian coor = new Cartesian(objname, Coor[0], Coor[1]);
                            mapCoords.Add(coor);

                            objname = "";
                            Coor = null;
                            Q = null;
                        }
                        else
                        {
                            string[] rawSP = rawline.Split(',');

                            if (rawSP.Length == 3)
                            {
                                Coor = Array.ConvertAll(rawSP, double.Parse);
                            }
                            else if (rawSP.Length == 4)
                            {
                                Q = Array.ConvertAll(rawSP, double.Parse);
                            }
                            else
                            {
                                objname = rawline;
                                objsName.Add(rawline);
                                nameIndex++;
                            }
                        }
                    }
                }
            }
        }

        private void ManageLOD(object sender, MouseButtonEventArgs e)
        {
            //We locate the Row's index that has been triggered, with VisualParent.
            e.Handled = true;

            System.Windows.Controls.Button addBt = sender as System.Windows.Controls.Button;

            if (addBt != null)
            {
                System.Windows.Controls.DataGridCell currCell = FindVisualParent<System.Windows.Controls.DataGridCell>(addBt);

                if (currCell != null)
                {
                    DataGridRow row = FindVisualParent<DataGridRow>(currCell);

                    if (row != null)
                    {
                        System.Windows.Controls.DataGrid dataGrid = FindVisualParent<System.Windows.Controls.DataGrid>(row);

                        int rowIndex = dataGrid.ItemContainerGenerator.IndexFromContainer(row);

                        if (displayedData[rowIndex] is rawData rawItem)
                        {
                            if (rawItem.lodData == null)
                            {
                                if (!string.IsNullOrEmpty(src_pth.Text))
                                {
                                    //We get the informations back from that dialog.
                                    AddLOD addLOD = new AddLOD(src_pth.Text);
                                    if (addLOD.ShowDialog() == true && addLOD.lodAdded)
                                    {
                                        string fullPth = Path.Combine(src_pth.Text, addLOD.modelName+ ".dff");

                                        if (File.Exists(fullPth))
                                        {
                                            rawItem.lodData = new LOD(lodCount, addLOD.modelName, rawItem.interior, addLOD.DrawDis, rawItem.flags);
                                            lodsName.Add(addLOD.modelName);
                                            rawItem.LOD = 1;
                                            lodCount++;
                                        }
                                        else
                                        {
                                            errShow($"The file {addLOD.modelName}.dff does not exist in {src_pth.Text}\\ directory.");
                                        }
                                    }
                                }
                                else
                                {
                                    errShow("No resource directory is selected.");
                                }
                            }
                        }
                        else if (displayedData[rowIndex] is LOD lodItem)
                        {
                            rawData parentRawData = dataColl.FirstOrDefault(item => item.lodData == lodItem);
                            if (parentRawData != null)
                            {
                                parentRawData.lodData = null;
                                lodsName.Remove(lodItem.modelName);
                                parentRawData.LOD = 0;
                            }
                            ShowIposData();
                        }
                        ShowIposData();
                    }
                }
            }
        }
        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {

            while (child != null)
            {
                if (child is T parent)
                {
                    return parent;
                }
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        private void ShowIposData()
        {
            //Update the data changes, and than apply by binding on the ui.
            displayedData.Clear();

            foreach (var item in dataColl)
            {
                item.isLOD = item.lodData != null ? 1 : 0;
                displayedData.Add(item);

                if (item.lodData != null)
                {
                    item.lodData.isLOD = 2;
                    displayedData.Add(item.lodData);
                }
            }

            iposData.ItemsSource = displayedData;
        }

        private void gta_sa_browse_Click(object sender, RoutedEventArgs e)
        {
            //We choose a folder that is a GTA SA root folder.
            using (FolderBrowserDialog gtasa_diag = new FolderBrowserDialog())
            {
                gtasa_diag.Description = "Select your GTA SA's root folder.";
                gtasa_diag.RootFolder = Environment.SpecialFolder.MyComputer;

                gtasa_diag.SelectedPath = @"C:\";

                if (gtasa_diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    gta_sa_pth.Text = gtasa_diag.SelectedPath;
                    writeINIsettings("GTA_SA_PATH", gtasa_diag.SelectedPath);
                }
            }
        }

        private void resrc_browse_Click(object sender, RoutedEventArgs e)
        {
            //We want a folder that'll holds all the data that required for an successful project creation.
            using (FolderBrowserDialog res_diag = new FolderBrowserDialog())
            {
                res_diag.Description = "Select your resources folder.";
                res_diag.RootFolder = Environment.SpecialFolder.MyComputer;

                res_diag.SelectedPath = @"C:\";

                if (res_diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    src_pth.Text = res_diag.SelectedPath;
                }
            }
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            //There is where we start to execute the creation of the project. 
            string GTA_SA_pth = gta_sa_pth.Text;
            string project_name = proj_name.Text;
            string modloader_pth = Path.Combine(GTA_SA_pth, "modloader");
            string gta_sa_bin = Path.Combine(GTA_SA_pth, "gta_sa.exe");

            //Had to do the, keep gating
            if (string.IsNullOrEmpty(GTA_SA_pth))
            {
                errShow("Please select your GTA SA root folder.");
            }
            else if (string.IsNullOrEmpty(project_name))
            {
                errShow("Please name the project.");
            }
            else if (string.IsNullOrEmpty(src_pth.Text))
            {
                errShow("Please select your folder that contains .dff, .txd, .col files.");
            }
            else if (!File.Exists(gta_sa_bin))
            {
                errShow("Grand Theft Auto San Andreas binary file not found.");
            }
            else if (!Directory.Exists(modloader_pth))
            {
                errShow("Modloader directory is missing.");
            }
            else
            {
                //There's where we validate all the resources. 
                bool isValid = validateResources();

                if (isValid)
                {
                    projectGenerating();
                }
            }
        }

        private bool validateResources()
        {
            string[] files = { };
            string[] fileExtensions = { ".dff", ".txd" };
            string srcPath = src_pth.Text;

            //We check if there's all the data pairs one .dff, .txd per object.
            if (!Directory.Exists(src_pth.Text))
            {
                errShow("Source path directory does not exist.");
                return false;
            }

            foreach (string ext in fileExtensions)
            {
                string[] currFiles = Directory.GetFiles(srcPath, $"*{ext}", SearchOption.AllDirectories).Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();
                var missingItems = objsName.Where(element => !currFiles.Contains(element)).ToArray();

                if (missingItems.Length > 0)
                {
                    string missingItemsMessage = string.Join(", ", missingItems);
                    errShow($"Missing items in the project: {missingItemsMessage}");
                    return false;
                }
            }

            string[] currCols = Directory.GetFiles(src_pth.Text, $"*.col", SearchOption.AllDirectories).Select(file => Path.GetFullPath(file)).ToArray();

            //We check if the .col files contains all the refenreces back to the file 'cause what matters to RenderWare GTA SA engine is not the actual file name, but rather what names holded by that data. 
            return readCOL(currCols);
        }

        private bool readCOL(string[] cols)
        {
            //Header size for the header data.
            int headerSize = 4;
            //We want to search for that "COL3" in the binary. 
            byte[] col3Header = new byte[] { 0x43, 0x4F, 0x4C, 0x33 };
            List<string> col3names = new List<string>();

            foreach (string colpth in cols)
            {
                using (FileStream colFS = new FileStream(colpth, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader colBR = new BinaryReader(colFS);
                    //I used the hit character technique, to get the header, everywhere even if it's not consistent 
                    int byteHeaderHit = 0;

                    while (colBR.BaseStream.Position < colBR.BaseStream.Length)
                    {
                        byte currByte = colBR.ReadByte();

                        if (currByte == col3Header[byteHeaderHit])
                        {
                            byteHeaderHit++;

                            if (byteHeaderHit == headerSize)
                            {
                                //'cause of the byte padding between the header that hold the type and the actual name of the collision data.
                                colBR.BaseStream.Seek(headerSize, SeekOrigin.Current);
                                col3names.Add(getCOL3name(colBR));
                                byteHeaderHit = 0;
                            }
                        }
                        else
                        {
                            byteHeaderHit = 0;
                        }
                    }
                }
            }

            var missingItems = objsName.Where(element => !col3names.Contains(element)).ToArray();

            if (missingItems.Length > 0)
            {
                string missingItemsMessage = string.Join(", ", missingItems);
                errShow($"Missing .col files or named improperly for: ({missingItemsMessage}).dff");
                return false;
            }

            return true;
        }

        private string getCOL3name(BinaryReader br)
        {
            //We just iterate and append character by character to the Stringbuild until it'll ends, when we hit a 0x00 byte. 
            StringBuilder col3name = new StringBuilder();

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                byte currByte = br.ReadByte();

                if (currByte == 0x00)
                {
                    break;
                }
                col3name.Append((char)currByte);
            }

            return col3name.ToString();
        }

        private void projectGenerating()
        {
            //We create the basic struct of a mod. 
            string projectName = proj_name.Text;
            string modloader = Path.Combine(gta_sa_pth.Text, "modloader");
            string modProject = Path.Combine(modloader, projectName);

            if (Directory.Exists(modProject))
            {
                errShow($"Mod exist with the name {projectName} in {modProject} already.");
            }
            else
            {
                string projectFullPth = Path.Combine(modProject, "data\\maps");

                if (!Directory.Exists(projectFullPth))
                {
                    Directory.CreateDirectory(projectFullPth);
                }
                else
                {
                    errShow($"{projectName} already exist in {projectFullPth}.");
                    return;
                }

                bool isGTADATAgenerated = generateGTADAT(projectName, modProject);

                if (isGTADATAgenerated)
                {
                    List<int> freeIDs = getFreeIDs(projectFullPth, modloader);

                    bool isIDEiplgenerated = generateIDEipl(freeIDs, projectFullPth, projectName);

                    if (isIDEiplgenerated)
                    {
                        string resources = src_pth.Text;

                        foreach (string file in Directory.GetFiles(resources))
                        {
                            string destFile = Path.Combine(modProject, Path.GetFileName(file));
                            File.Copy(file, destFile, true);
                        }
                        successMessageShow($"Mod '{projectName}' successfully created at: {modProject}.");
                    }
                }
            }
        }

        private bool generateGTADAT(string projectName, string modProject)
        {
            //We generate gta.dat template from embedded resource. 
            var datAsm = Assembly.GetExecutingAssembly();
            var resourceName = "IPOS.gta.dat";
            string GTADATpth = Path.Combine(modProject, "data\\gta.dat");

            if (!File.Exists(GTADATpth))
            {
                using (FileStream fs = File.Create(GTADATpth)) { };
            }

            try
            {
                using (Stream datStream = datAsm.GetManifestResourceStream(resourceName))
                using (StreamReader datRD = new StreamReader(datStream))
                using (StreamWriter datWR = new StreamWriter(GTADATpth))
                {
                    string line;

                    while ((line = datRD.ReadLine()) != null)
                    {
                        datWR.WriteLine(line);

                        if (line.Trim() == "# Object types")
                        {
                            datWR.WriteLine(datRD.ReadLine());
                            datWR.WriteLine($"IDE DATA\\MAPS\\{projectName}.IDE");
                        }
                        else if (line.Trim() == "SPLASH loadsc2")
                        {
                            datWR.WriteLine(datRD.ReadLine());
                            datWR.WriteLine($"IPL DATA\\MAPS\\{projectName}.IPL");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errShow($"Error occurred while creating gta.dat: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return false;
            }

            return true;
        }

        private List<int> getFreeIDs(string dest, string modloaderPth)
        {
            //We have to get some ids for out project so, there is an predetermined lists of "some" free id, that's free in the vanilla game. 
            List<int> IDs = new List<int>();
            List<int> excludedIDs = getAllExcludedIDs(modloaderPth);
            int idCount = objsName.Count + lodsName.Count;
            int freeIdsCount = freeIdRanges.Count;

            Random pickRange = new Random();

            var (start, end) = (0, 0);
            int rangeIndex = pickRange.Next(freeIdsCount);
            (start, end) = freeIdRanges[rangeIndex];

            for (int i = 0; i < idCount; i++)
            {
                if (!excludedIDs.Contains(start))
                {
                    IDs.Add(start);
                }
                start++;

                if (start > end)
                {
                    rangeIndex = pickRange.Next(freeIdsCount);
                    (start, end) = freeIdRanges[rangeIndex];
                }
            }

            return IDs;
        }

        private bool generateIDEipl(List<int> freeIDs, string fullPth, string projectname)
        {
            //We generate the .ide, .ipl files from the datas that's been given from dataColl.
            string idePth = Path.Combine(fullPth, $"{projectname}.ide");
            string iplPth = Path.Combine(fullPth, $"{projectname}.ipl");
            using (FileStream ideFs = File.Create(idePth)) { };
            using (FileStream iplFs = File.Create(iplPth)) { };

            try
            {
                using (StreamWriter ideRW = new StreamWriter(idePth))
                using (StreamWriter iplRW = new StreamWriter(iplPth))
                {
                    ideRW.WriteLine("objs");
                    iplRW.WriteLine("inst");

                    int idCount = 0;

                    for (int i = 0; i < dataColl.Count; i++)
                    {
                        string interiorS = dataColl[i].interior == true ? "1" : "0";

                        if (dataColl[i].LOD == 0)
                        {
                            ideRW.WriteLine($"{freeIDs[idCount]}, {dataColl[i].modelName}, {dataColl[i].textureName}, {dataColl[i].drawDis}, {dataColl[i].flags}");
                            iplRW.WriteLine($"{freeIDs[idCount]}, {dataColl[i].modelName}, {interiorS}, {dataColl[i].x}, {dataColl[i].y}, {dataColl[i].z}, {dataColl[i].qX}, {dataColl[i].qY}, {dataColl[i].qZ}, {dataColl[i].qW}, -1");
                        }
                        else
                        {
                            ideRW.WriteLine($"{freeIDs[idCount]}, {dataColl[i].modelName}, {dataColl[i].textureName}, {dataColl[i].drawDis}, {dataColl[i].flags}");
                            iplRW.WriteLine($"{freeIDs[idCount]}, {dataColl[i].modelName}, {interiorS}, {dataColl[i].x}, {dataColl[i].y}, {dataColl[i].z}, {dataColl[i].qX}, {dataColl[i].qY}, {dataColl[i].qZ}, {dataColl[i].qW}, {dataColl[i].LOD}");
                            idCount++;
                            ideRW.WriteLine($"{freeIDs[idCount]}, {dataColl[i].lodData.modelName}, {dataColl[i].textureName}, {dataColl[i].lodData.drawDis}, {dataColl[i].flags}");
                            iplRW.WriteLine($"{freeIDs[idCount]}, {dataColl[i].lodData.modelName}, {interiorS}, {dataColl[i].x}, {dataColl[i].y}, {dataColl[i].z}, {dataColl[i].qX}, {dataColl[i].qY}, {dataColl[i].qZ}, {dataColl[i].qW}, -1");
                        }
                        idCount++;
                    }
                    ideRW.WriteLine("end");
                    iplRW.WriteLine("end");

                    dataColl.Clear();
                    objsName.Clear();
                    lodsName.Clear();
                    currLod = "";
                }
            }
            catch (Exception ex)
            {
                errShow($"Error occurred while creating {idePth} and {iplPth}: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return false;
            }

            return true;
        }

        private List<int> getAllExcludedIDs(string modloaderPth)
        {
            //We get the lists of the already used ids by get it from the files from modloader folder.
            string[] ideFiles = Directory.GetFiles(modloaderPth, "*.ide", SearchOption.AllDirectories);
            string[] iplFiles = Directory.GetFiles(modloaderPth, "*.ipl", SearchOption.AllDirectories);

            HashSet<int> excludedIDs = new HashSet<int>();
            getIDs(excludedIDs, ideFiles);
            getIDs(excludedIDs, iplFiles);

            return excludedIDs.ToList();
        }

        private void getIDs(HashSet<int> excludedIDs, string[] filePaths)
        {
            //We parse the .ide, .ipl file and get the ids.
            for (int i = 0; i < filePaths.Length; i++)
            {
                using (FileStream fs = new FileStream(filePaths[i], FileMode.Open, FileAccess.Read))
                using (StreamReader sr = new StreamReader(fs))
                {
                    string header = sr.ReadLine();
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim() == "end")
                        {
                            break;
                        }
                        string[] spLine = line.Split(',');
                        int id = int.Parse(spLine[0]);

                        excludedIDs.Add(id);
                    }
                }
            }
        }

        //Error show dialog
        private void errShow(string message)
        {
            message errmsg = new message(message, true);
            errmsg.ShowDialog();
        }

        //Success show dialog
        private void successMessageShow(string message)
        {
            message successmsg = new message(message, false);
            successmsg.ShowDialog();
        }

        //Free ids intervals from the vanilla GTA SA.
        private readonly List<(int, int)> freeIdRanges = new List<(int, int)>
        {
            (2, 6), (8, 0), (42, 0), (65, 0), (74, 0),
            (86, 0), (119, 0), (149, 0), (208, 0),
            (265, 273), (289, 0), (329, 0), (332, 0),
            (340, 0), (382, 383), (398, 399), (612, 614),
            (662, 663), (665, 668), (699, 0), (793, 799),
            (907, 909), (965, 0), (999, 0), (1194, 1206),
            (1326, 0), (1573, 0), (1699, 0), (2883, 2884),
            (3136, 3166), (3176, 3177), (3179, 3186), (3188, 3192),
            (3194, 3213), (3215, 3220), (3222, 3240), (3245, 0),
            (3247, 3248), (3251, 0), (3254, 0), (3266, 0),
            (3348, 3349), (3416, 0), (3429, 0), (3610, 3611),
            (3784, 0), (3870, 3871), (3883, 0), (3889, 0),
            (3974, 0), (4542, 4549), (4763, 4805), (5085, 0),
            (5090, 5104), (5376, 5389), (5683, 5702), (6011, 6034),
            (6254, 0), (6258, 6279), (6348, 0), (6526, 6862),
            (7393, 7414), (7974, 7977), (9194, 9204), (9268, 0),
            (9479, 9481), (10311, 10314), (10745, 10749), (11418, 11419),
            (11682, 12799), (13564, 13589), (13668, 13671), (13891, 14382),
            (14529, 0), (14555, 0), (14557, 0), (14644, 14649),
            (14658, 14659), (14696, 14698), (14729, 14734), (14766, 14769),
            (14857, 0), (14884, 0), (14899, 0), (14904, 15024),
            (15065, 15999), (16791, 16999), (17475, 17499), (17975, 0),
            (17977, 0), (17979, 17999), (18037, 0), (18103, 0),
            (18106, 18108), (18110, 18111), (18113, 18199), (18631, 19999)
        };
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace UDT_Term_FFT
{
    public partial class AppDGVExample : Form
    {
        public bool isXML_File_Error { get; set; }       // No error during XML decoding. Error if XML fails or file is missing.
        public ConfigurationZZ ConfigData;               // This is class only
        private string filePath;                         // File pathe where XML is stored. 
        public AppDGVExample()
        {
            InitializeComponent();
            isXML_File_Error = false;


            //XML_DataGridView_Example_1A();
            XML_ListView_Example_1H_DataSource();
            this.Show();
        }
        List<ConfigurationZZ> config;                           // List generic type
        Collection<ConfigurationZZ> configCollection;           // Collection generic type
        private bool _isLoading = true;
        #region //========================================================== OnSelectedIndexChanged
        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            #region//-----------------------------------Below is based on DataSource, it does not need loop routine. This work only on Example_1H.
            if ((!_isLoading) && (sender is ListBox))
            {
                ListBox lb = sender as ListBox;
                if (lb.SelectedItem is ConfigurationZZ)
                {
                    ConfigurationZZ configxx = lb.SelectedItem as ConfigurationZZ;
                    MessageBox.Show(string.Format("You have selected - CompanyID: {0}", configxx.CompanyID));
                }
            }
            #endregion

            #region//-----------------------------------Below is based on helper class, it does not need loop routine. This work only on Example_1G Only.
            /*
            if (sender is ListBox)
            {
                ListBox lb = sender as ListBox;
                if (lb.SelectedItem is ListItemData<ConfigurationZZ>)
                {
                    ListItemData<ConfigurationZZ> lid = lb.SelectedItem as ListItemData<ConfigurationZZ>;
                    MessageBox.Show(string.Format("You have selected - CompanyID: {0}", lid.ValueMember.CompanyID));
                }
            }
            */
            #endregion

            #region //-----------------------------------Below is search and identify matching object and select index, this is old but working method. This work only for Example_1F and ealier code. 
            /*
            if (sender is ListBox)
            {
                ListBox lb = sender as ListBox;
                string sData = lb.SelectedItem.ToString();
                int index;
                for (int i = 0; i < config.Count; i++)
                {
                    index = sData.IndexOf(config[i].CompanyString);     // Seek for match and return index if found. 
                    if (index>0)                                        // -1 if not found, 0 = null. 
                        MessageBox.Show(string.Format("You have selected - CompanyID: {0}", config[i].CompanyID));
                }

                // You could also use a For Each loop
                //foreach (var config in configurations)
                //{
                //     if (config.CompanyString.Equals(lb.SelectedItem))
                //          MessageBox.Show(string.Format("You have selected - CompanyID: {0}", configurations[i].CompanyID));
                //}

                // Or even linq
                //var selected = (from config in configurations where config.CompanyString.Equals(lb.SelectedItem) select config).FirstOrDefault();
                //if (selected != null)
                //     MessageBox.Show(string.Format("You have selected - CompanyID: {0}", configurations[i].CompanyID));

            }
            */
            #endregion

        }
        #endregion

        #region //========================================================== XML_ListView_Example_1H_DataSource, same as Example 1F, rely on helper class ListItemData<T>
        // But using an array (Example_1D) is so archaic compared to using an enumerable [or a collection], i.e. - A list:
        private void XML_ListView_Example_1H_DataSource()
        {
            filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();      // Setup FilePath to Resource Folder Box.
            filePath += "Resources//XMLConfig1C.xml";

            if (File.Exists(filePath))
            {
                config = DeserializeFromFile2<List<ConfigurationZZ>>(filePath);
            }
            else
            {
                if (config == null)
                {
                    config = new List<ConfigurationZZ>();
                    for (uint i = 0; i < 8; i++)
                        config.Add(new ConfigurationZZ() { CompanyID = (uint)i, CompanyString = string.Format("ZMDI{0}", i), Test1 = i * 20, Test2 = i * i });
                }
                SerializeToFile<List<ConfigurationZZ>>(config, filePath);
            }
            if (config != null)
            {
                //listBox1.SelectedIndexChanged -= OnSelectedIndexChanged;
                listBox1.DataSource = config;
                listBox1.DisplayMember = "CompanyString";

                //for (int i=0; i< config.Count;i++)
                //{
                //    listBox1.Items.Add("Company Name: " + config[i].CompanyString + " | Company ID: " + config[i].CompanyID);
//                }

                _isLoading = false;
                //listBox1.SelectedIndexChanged += OnSelectedIndexChanged;
            }
        }
        #endregion


        //###############################Old Code but keep for future reference

        #region //========================================================== XML_ListView_Example_1G_Helper, same as Example 1F, rely on helper class ListItemData<T>
        // But using an array (Example_1D) is so archaic compared to using an enumerable [or a collection], i.e. - A list:
        private void XML_ListView_Example_1G_Helper()
        {
            filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();      // Setup FilePath to Resource Folder Box.
            filePath += "Resources//XMLConfig1C.xml";

            if (File.Exists(filePath))
            {
                config = DeserializeFromFile2<List<ConfigurationZZ>>(filePath);
            }
            else
            {
                if (config == null)
                {
                    config = new List<ConfigurationZZ>();
                    for (uint i = 0; i < 8; i++)
                        config.Add(new ConfigurationZZ() { CompanyID = (uint)i, CompanyString = string.Format("ZMDI{0}", i), Test1 = i * 20, Test2 = i * i });
                }
                SerializeToFile<List<ConfigurationZZ>>(config, filePath);
            }
            if (config != null)
            {
                // **************************************************  Using the Add Methods **************************************************
                //for (int i = 0; i < configurations.Count; i++)
                //     listBox1.Items.Add(new ListItemData<ConfigurationX>(configurations[i].CompanyString, configurations[i]));

                // You could also use a for each loop
                //foreach (var config in configurations)
                //     listBox1.Items.Add(new ListItemData<ConfigurationX>(config.CompanyString, config));
                // ****************************************************************************************************************************

                // ***************************************** Using the AddRange Methods with an Array *****************************************
                // Seed array for the addrange method.
                //object[] items = new object[configurations.Count];

                //for (int i = 0; i < configurations.Count; i++)
                //     items[i] = new ListItemData<ConfigurationX>(configurations[i].CompanyString, configurations[i]);

                // You could also use a for each loop
                //int index = 0;
                //foreach (var config in configurations)
                //{
                //     items[index] = new ListItemData<ConfigurationX>(config.CompanyString, config);
                //     index++;
                //}

                // You could also use a linq statement
                // items = (from config in configurations select new ListItemData<ConfigurationX>(config.CompanyString, config)).ToArray();
                // ****************************************************************************************************************************

                // ************************************* Using the AddRange Methods with an Intermediary **************************************
                // Intermediary collection for the addrange method.
                //List<ListItemData> items = new List<ListItemData>();

                //for (int i = 0; i < configurations.Count; i++)
                //     items.Add(new ListItemData<ConfigurationX>(configurations[i].CompanyString, configurations[i]));

                // You could also use a for each loop
                //foreach (var config in configurations)
                //     items.Add(new ListItemData<ConfigurationX>(config.CompanyString, config));

                // You could also use a linq statement
                // items = (from config in configurations select new ListItemData<ConfigurationX>(config.CompanyString, config));

                //listBox1.Items.AddRange(items.ToArray());
                // ****************************************************************************************************************************

                // But again we do not need to use an intermediary
                listBox1.Items.AddRange((from configxx in config select new ListItemData<ConfigurationZZ>(configxx.CompanyString, configxx)).ToArray());
            }
        }
        #endregion

        #region //========================================================== XML_ListView_Example_1F, same as Example 1E, except AddRange is used instead of Range. This is List<T> method, this solution does not need intermediary code.
        // But using an array (Example_1D) is so archaic compared to using an enumerable [or a collection], i.e. - A list:
        private void XML_ListView_Example_1F()
        {
            filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();      // Setup FilePath to Resource Folder Box.
            filePath += "Resources//XMLConfig1C.xml";

            if (File.Exists(filePath))
            {
                config = DeserializeFromFile2<List<ConfigurationZZ>>(filePath);
            }
            else
            {
                if (config == null)
                {
                    config = new List<ConfigurationZZ>();
                    for (uint i = 0; i < 8; i++)
                        config.Add(new ConfigurationZZ() { CompanyID = (uint)i, CompanyString = string.Format("ZMDI{0}", i), Test1 = i * 20, Test2 = i * i });
                }
                SerializeToFile<List<ConfigurationZZ>>(config, filePath);
            }
            // Wow, this is amazing what generic can do with this!!!!!!
            if (config != null)
            {
                // We don't need to use an intermediary.
                listBox1.Items.AddRange((from configxx in config select "Company Name: " + configxx.CompanyString + " | Company ID: " + configxx.CompanyID).ToArray());
            }
        }
        #endregion

        #region //========================================================== XML_ListView_Example_1E, same as Example 1D, except AddRange is used instead of Range. This is List<T> method, better since it enumerable approach.
        // But using an array (Example_1D) is so archaic compared to using an enumerable [or a collection], i.e. - A list:
        private void XML_ListView_Example_1E()
        {
            filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();      // Setup FilePath to Resource Folder Box.
            filePath += "Resources//XMLConfig1C.xml";

            if (File.Exists(filePath))
            {
                config = DeserializeFromFile2<List<ConfigurationZZ>>(filePath);
            }
            else
            {
                if (config == null)
                {
                    config = new List<ConfigurationZZ>();
                    for (uint i = 0; i < 8; i++)
                        config.Add(new ConfigurationZZ() { CompanyID = (uint)i, CompanyString = string.Format("ZMDI{0}", i), Test1 = i * 20, Test2 = i * i });
                }
                SerializeToFile<List<ConfigurationZZ>>(config, filePath);
            }
            // But using an array is so archaic (example 1D) compared to using an enumerable [or a collection], i.e. - A list:
            if (config != null)
            {

                List<object> items = new List<object>();        // Intermediary collection for the addrange method.

                for (int i = 0; i < config.Count; i++)
                    items.Add("Company Name: " + config[i].CompanyString + " | Company ID: " + config[i].CompanyID);
                listBox1.Items.AddRange(items.ToArray());
            }
        }
        #endregion

        #region //========================================================== XML_ListView_Example_1D, same as Example 1C, except AddRange is used instead of Range. This is array[] method (not the best)
        private void XML_ListView_Example_1D()
        {
            filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();      // Setup FilePath to Resource Folder Box.
            filePath += "Resources//XMLConfig1C.xml";

            if (File.Exists(filePath))
            {
                config = DeserializeFromFile2<List<ConfigurationZZ>>(filePath);
            }
            else
            {
                if (config == null)
                {
                    config = new List<ConfigurationZZ>();
                    for (uint i = 0; i < 8; i++)
                        config.Add(new ConfigurationZZ() { CompanyID = (uint)i, CompanyString = string.Format("ZMDI{0}", i), Test1 = i * 20, Test2 = i * i });
                }
                SerializeToFile<List<ConfigurationZZ>>(config, filePath);
            }

            if (config != null)
            {

                object[] items = new object[config.Count];          // Seed array for the addrange method.

                for (int i = 0; i < config.Count; i++)
                    items[i] = "Company Name: " + config[i].CompanyString + " | Company ID: " + config[i].CompanyID;

                listBox1.Items.AddRange(items);
            }
            // You could also use a for each loop
            //int index = 0;
            //foreach (var config in configurations)
            //{
            //     items[index] = config.CompanyString;
            //     index++;
            //}

            // You could also use a linq statement
            // items = (from config in configurations select config.CompanyString).ToArray();

        }
        #endregion

        #region //========================================================== XML_ListView_Example_1C, come from expert exchange XML/listbox view no longer need override ToString 
        private void XML_ListView_Example_1C()
        {
            filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();      // Setup FilePath to Resource Folder Box.
            filePath += "Resources//XMLConfig1C.xml";

            if (File.Exists(filePath))
            {
                config = DeserializeFromFile2<List<ConfigurationZZ>>(filePath);
            }
            else
            {
                if (config == null)
                {
                    config = new List<ConfigurationZZ>();
                    for (uint i = 0; i < 8; i++)
                        config.Add(new ConfigurationZZ() { CompanyID = (uint)i, CompanyString = string.Format("ZMDI{0}", i), Test1 = i * 20, Test2 = i * i });
                }
                SerializeToFile<List<ConfigurationZZ>>(config, filePath);
            }

            if (config != null)
            {
                for (int i = 0; i < config.Count; i++)
                    listBox1.Items.Add("Company Name: " + config[i].CompanyString + " | Company ID: " + config[i].CompanyID);
            }

            // You could also use a for each loop
            //foreach (var config in configurations)
            //     listBox1.Items.Add(config.CompanyString);

        }
        #endregion

        #region //========================================================== XML_ListView_Example_Collection_1A, experiment with collection<T> instead of List<T>, also worked well. 


        private void XML_ListView_Example_Collection_1A()
        {
            filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();      // Setup FilePath to Resource Folder Box.
            filePath += "Resources//XMLConfigCollection1A.xml";

            if (File.Exists(filePath))
            {
                configCollection = DeserializeFromFile2<Collection<ConfigurationZZ>>(filePath);
            }
            else
            {
                if (configCollection == null)
                {
                    configCollection = new Collection<ConfigurationZZ>();
                    configCollection.Add(new ConfigurationZZ() { CompanyID = (uint)10, CompanyString = string.Format("ZMDI{0}", 1), Test1 = 20, Test2 = 40 });
                }
                SerializeToFile<Collection<ConfigurationZZ>>(configCollection, filePath);
            }

            if (configCollection != null)
            {
                listBox1.Items.Add("Company Name: " + configCollection[0].CompanyString + " | Company ID: " + configCollection[0].CompanyID);

                // You could also use a for each loop
                //foreach (var config in configurations)
                //     listBox1.Items.Add(config.CompanyString);
            }

        }
        #endregion

        #region //========================================================== XML_ListView_Example_1B, come from expert exchange XML/listbox view but depend on ToString. 

        private void XML_ListView_Example_1B()
        {
            filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();      // Setup FilePath to Resource Folder Box.
            filePath += "Resources//XMLConfig1B.xml";
            
            if (File.Exists(filePath))
            {
                config = DeserializeFromFile2<List<ConfigurationZZ>>(filePath);
            }
            else
            {
                if (config == null)
                {
                    config = new List<ConfigurationZZ>();
                    for (uint i = 0; i < 2; i++)
                        config.Add(new ConfigurationZZ() { CompanyID = (uint)i, CompanyString = string.Format("ZMDI{0}", i), Test1 = i*20, Test2 = i*i });
                }
                SerializeToFile<List<ConfigurationZZ>>(config, filePath);
            }

            if (config != null)
                listBox1.DataSource = config;

        }
        #endregion

        #region //========================================================== XML_DataGridView_Example_1A This is DGV early experiments, working under Riscy Study.
        private void XML_DataGridView_Example_1A()
        {
            filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();      // Setup FilePath to Resource Folder Box.
            filePath += "Resources//XMLConfigZZ.xml";

            List<ConfigurationZZ> config;                                                   // This is List<T> 
            BindingList<ConfigurationZZ> configb = new BindingList<ConfigurationZZ>();
            ConfigData = new ConfigurationZZ();

            if (File.Exists(filePath))
            {
                // Deserialize the existing list or create an empty one if none exists yet.
                //config = DeserializeFromFile<List<Configuration>>(filePath);
                configb = DeserializeFromFile2<BindingList<ConfigurationZZ>>(filePath);
                if (configb == null)
                    isXML_File_Error = true;
                ConfigData.CompanyID = configb[0].CompanyID;
                ConfigData.CompanyString = configb[0].CompanyString;
                ConfigData.Test1 = configb[0].Test1;
                ConfigData.Test2 = configb[0].Test2;
                ConfigData.Test3 = configb[0].Test3;
            }
            else
            {
                // Serialize the existing list or create an empty one if none exists yet.  
                ConfigData.ConfigurationDefault();                 // Default Data
                config = new List<ConfigurationZZ>();                // Create list object
                config.Add(ConfigData);                             // Add object to list
                SerializeToFile(config, filePath);
            }
            //-----------------------------------------------------------------
            dataGridView1.DataSource = configb;
            //dataGridView1.Update();
            //dataGridView1.Refresh();
        }
        #endregion

        //###############################End of Demo/Old Code

        #region //-------------------------------------------------------------------SerializeToFile
        // Source from: http://stackoverflow.com/questions/28499781/deserializing-xml-into-c-sharp-so-that-the-values-can-be-edited-and-re-serialize
        // An 'System.IO.FileNotFoundException' exception is thrown but handled by the XmlSerializer, so if you just ignore it everything should continue on fine.
        // Method-1 This is better due to \r\n for each XML statement but 
        private  void SerializeToFile<T>(T item, string xmlFileName)
        {
            if (typeof(T).IsSerializable)       // Check before applying, in case class has missing [Serializable]
            {
                using (FileStream stream = new FileStream(xmlFileName, FileMode.Create, FileAccess.Write))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stream, item);
                }
            }
        }
        #endregion


        #region //-------------------------------------------------------------------DeserializeFromFile2
        // This is better solution since there is no IO filename exception error. 
        private  T DeserializeFromFile2<T>(string filenme) where T : class
        {
            T result = null;
            if (typeof(T).IsSerializable)            // Check before applying, in case class has missing [Serializable]
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (XmlTextReader reader = new XmlTextReader(filenme))
                {
                    try
                    {
                        result = (T)serializer.Deserialize(reader);
                        //Console.WriteLine("---INFO: Deserialization successful! Got string: \n{0}", result);
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("###ERR: Failed to deserialize object!!");
                    }
                }
            }
            return result;
            //return default(T); or this method, not sure which one best....! (see expert exchange)
        }
        #endregion




        #region ---------------------------------------------------------------------Configuration Class --
        // Set of class object's of member being transformed into element in XML.
        // When change take place, ie added element (member), must delete old XMLConfig file. 
        [Serializable]
        public class ConfigurationZZ
        {
            // This is new style Getter and Setter for modern .NET typestyle. 
            // ==================================================================Variable as part of XML Configuration Data


            // ==================================================================Getter/Setter
            public uint CompanyID { get; set; }
            public string CompanyString { get; set; }
            public uint Test1 { get; set; }
            public uint Test2 { get; set; }
            public uint Test3 { get; set; }

            // ==================================================================constructor
            public ConfigurationZZ()
            {
                // Leave blank, do not install default data here.
            }

            public void ConfigurationDefault()
            {
                CompanyID = 10;
                CompanyString = "ZMDI";
                Test1 = 9678;
                Test2 = 34453;
                Test3 = 12981;
            }

            /*
            public override string ToString()
            {
                return string.Format("ID: {0}; String: {1}; Test1: {2}; Test2: {3}; Test4: {4}", CompanyID, CompanyString, Test1, Test2, Test3);
            }
            */

        }
        #endregion

        #region //############################### Helper Class for Example 1F only. 
        class ListItemData<T>
        {
            public T ValueMember { get; private set; }
            public string DisplayMember { get; private set; }

            private ListItemData() {; }

            public ListItemData(string displayMember, T valueMember)
            {
                DisplayMember = displayMember;
                ValueMember = valueMember;
            }

            public override string ToString()
            {
                return DisplayMember;
            }
        }
        #endregion
    }
}


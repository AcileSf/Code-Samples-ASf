using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.ComponentModel;
using PO_FileRetrieverUI.Classes;
using MessageBox = System.Windows.MessageBox;
using CheckBox = System.Windows.Controls.CheckBox;
using System.IO.Compression;
using Microsoft.IdentityModel.Tokens;

namespace PO_FileRetrieverUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AppWindow : Window
    {
        private readonly FileType[] flt = [new("PDF", "pdf"), new("DWG", "dwg"), new("STEP", "step,stp")];
        private readonly PackageType[] pkg = [
            new("PO", "podetail", "pono", "SELECT tit.itemcode, tir.revision, tit.BOMID FROM nita.dbo.podetail tpo INNER JOIN nita.dbo.items tit ON tit.itemid = tpo.itemid LEFT JOIN nita.dbo.itemrevision tir ON tir.itemrevisionid = tpo.itemrevisionid WHERE pono="),
            new("Quote", "qpoheader", "qpono", "SELECT tit.itemcode, tir.revision, tit.BOMID FROM nita.dbo.qpoheader qpoh INNER JOIN nita.dbo.qpodetail qpod ON qpoh.qpoid = qpod.qpoid INNER JOIN nita.dbo.items tit ON tit.itemid = qpod.itemid INNER JOIN nita.dbo.itemrevision tir ON tir.itemrevisionid = qpod.itemrevisionid WHERE qpono="),
        ];


        // FULL SQL QUERY TO ACCESS CHILDREN OF ASSEMBLIES THROUGH THEIR BOM-ID NUMBERS
        //SELECT tit.itemcode, tir.revision, tit.BOMID FROM [nita].[dbo].[BOMDetail] tbom
        //INNER JOIN nita.dbo.items tit on tit.itemid = tbom.itemid
        //LEFT JOIN nita.dbo.itemrevision tir ON tir.itemrevisionid = tbom.itemrevisionid WHERE tbom.BomId = 191622


        //$"SELECT tit.itemcode, tir.revision, tit.BOMID {retrieveSQL} {column}="

        private PackageType selectedPkg = new("", "", "", "");

        private readonly string DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";

        private List<string> missingMessages = [];

        public AppWindow()
        {
            if (!PrextraDB.Connect(3)) ConnectionIssueExit();
            InitializeComponent();

            Rb_0.SetResourceReference(ContentProperty, pkg[0].Name);
            Rb_0.Tag = pkg[0].Name;
            Rb_1.SetResourceReference(ContentProperty, pkg[1].Name);
            Rb_1.Tag = pkg[1].Name;

            ChkB_0.Content = flt[0].Description;
            ChkB_1.Content = flt[1].Description;
            ChkB_2.Content = flt[2].Description;

            Rb_0.IsChecked = true;
            TbSaveLoc.Text = DefaultPath;
        }


        /// <summary>
        /// Locks in all the user's choices and inputs on the UI, displays errors if a field is missing and calls the required functions.
        /// </summary>
        private void ClickConfirm(object sender, RoutedEventArgs e)
        {
            #region Inputs Validation and testing
            // Checks if a PO or Quote number is entered in the textbox and assigns it to a variable or displays an error message
            if (string.IsNullOrEmpty(TbPkgNumber.Text))
            {
                _ = MessageBox.Show(GenFnct.GetFromDictionary("MissNbr_part1") + " " + GenFnct.GetFromDictionary(selectedPkg.Name) + " " + GenFnct.GetFromDictionary("MissNbr_part2"), GenFnct.GetFromDictionary("MissNbrTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Checks if Save Location path is valid, assigns it to a variable or displays an error message if path invalid
            string savePath = TbSaveLoc.Text;
            if (!Directory.Exists(savePath))
            {
                _ = MessageBox.Show($"{savePath} " + GenFnct.GetFromDictionary("ChooseValidLoc"), GenFnct.GetFromDictionary("FolderNotExist"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Checks if at least one file type is selected or else displays an error message
            string?[] checkedBoxes = ChkB_G0.Children.OfType<CheckBox>().Where(x => x.IsChecked == true).Select(x => x.Content.ToString()).ToArray();
            if (checkedBoxes.Length == 0)
            {
                _ = MessageBox.Show(GenFnct.GetFromDictionary("FileTypeSlct"), GenFnct.GetFromDictionary("MissSlct"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            #endregion

            bool flagChildren = false;
            if (Cb_Children.IsChecked == true)
            {
                flagChildren = true;
            }

            missingMessages = [];   // To reset the missing files log after every iteration

            // If the input number exists in the SQL Databases, the ZIP generating sequence is launched, otherwise an error message is displayed
            switch (PrextraDB.PkgNbrChecker(TbPkgNumber.Text, selectedPkg.SQLExist))
            {
                case true:

                    List<FileType> selectedFlt = flt.Where(x => checkedBoxes.Any(y => y == x.Description)).ToList();
                    List<ItemEx>? selectedItems = PrextraDB.RetrievePackageDetails(selectedPkg.SQLRetrieve + TbPkgNumber.Text, flagChildren);
                    List<Item> finalItems = SortingDuplicates(selectedItems);
                    if (finalItems is null) ConnectionIssueExit();
                    else
                    {
                        LV_Clear();
                        GenerateZipFromPackage(savePath, TbPkgNumber.Text, selectedFlt, finalItems);
                        if (missingMessages.Count > 0)
                        {
                            missingMessages.Insert(0, GenFnct.GetFromDictionary("MissingMSG") + $" {selectedPkg.Name}-{TbPkgNumber.Text}:");
                            LV_Log.ItemsSource = missingMessages;
                        }
                        else LV_Log.Items.Add(GenFnct.GetFromDictionary("NoMissingMSG") + $" {selectedPkg.Name}-{TbPkgNumber.Text}.");

                        ResetAction();
                    }
                    break;
                case false:
                    _ = MessageBox.Show(GenFnct.GetFromDictionary("NbrInvalid_part1") +" "+ GenFnct.GetFromDictionary(selectedPkg.Name) +" "+ GenFnct.GetFromDictionary("NbrInvalid_part2"), GenFnct.GetFromDictionary("NbrNotFound"), MessageBoxButton.OK, MessageBoxImage.Error);
                    TbPkgNumber.Clear();
                    TbPkgNumber.Focus();
                    break;
                default:
                    ConnectionIssueExit();
                    break;
            }
        }


        /// <summary>
        /// Calls the ResetAction function when Reset button is clicked + clears the log of missing files from the UI (without unchecking the box if it's already checked) + resets Save Path to user's download folder if the existing field is empty or invalid
        /// </summary>
        private void ClickReset(object sender, RoutedEventArgs e)
        {
            ResetAction();
            LV_Clear();
            if (!Directory.Exists(TbSaveLoc.Text) || TbSaveLoc.Text.IsNullOrEmpty()) TbSaveLoc.Text = DefaultPath;
        }


        /// <summary>
        /// Only clears PO/Quote input textbox field, children selection checkbox and all file types checkboxes
        /// </summary>
        private void ResetAction()
        {
            TbPkgNumber.Clear();
            TbPkgNumber.Focus();
            Cb_Children.IsChecked = false;
            ChkB_0.IsChecked = false;
            ChkB_1.IsChecked = false;
            ChkB_2.IsChecked = false;
        }


        /// <summary>
        /// Displays the placeholder text in the PO/Quote input textbox if no number is entered
        /// </summary>
        private void TbPkgNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            LblPkg.Visibility = string.IsNullOrEmpty(TbPkgNumber.Text) ? Visibility.Visible : Visibility.Hidden;
            PoQuote2.Visibility = string.IsNullOrEmpty(TbPkgNumber.Text) ? Visibility.Visible : Visibility.Hidden;
        }


        /// <summary>
        /// Forbids PO/Quote input textbox from accepting non-numeric characters
        /// </summary>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        /// <summary>
        /// Displays the placeholder text in the Save Location textbox if no input is entered
        /// </summary>
        private void TbSaveLoc_TextChanged(object sender, TextChangedEventArgs e) => LblSaveLoc.Visibility = string.IsNullOrEmpty(TbSaveLoc.Text) ? Visibility.Visible : Visibility.Hidden;


        /// <summary>
        /// Opens the folder browsing dialog when Browse Button is clicked
        /// </summary>
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Create FolderBrowserDialog
            FolderBrowserDialog dlg = new()
            {
                InitialDirectory = DefaultPath          // Take user to their own Downloads folder by default
            };
            DialogResult dlgResult = dlg.ShowDialog();

            if (dlgResult == System.Windows.Forms.DialogResult.OK) TbSaveLoc.Text = dlg.SelectedPath;
        }


        /// <summary>
        /// Initiates Window closing event when the Exit Button is clicked
        /// </summary>
        private void ClickExit(object sender, RoutedEventArgs e) => this.Close();


        /// <summary>
        /// Closes the SQL connection (if it is already open) when X or the Exit Button is clicked
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e) => PrextraDB.Close();


        /// <summary>
        /// Dynamically changes the title and the placeholder text of the input number box whether PO or Quote radio button is selected
        /// </summary>
        private void Rb_Selection(object sender, RoutedEventArgs e)
        {
            selectedPkg = pkg.First(x => x.Name == ((System.Windows.Controls.RadioButton)sender).Tag.ToString());
            PoQuote.SetResourceReference(ContentProperty, selectedPkg.Name);
            PoQuote2.SetResourceReference(ContentProperty, selectedPkg.Name);
        }


        /// <summary>
        /// Calls the closing of the application if the connection is unavailable
        /// </summary>
        private void ConnectionIssueExit()
        {
            _ = MessageBox.Show(GenFnct.GetFromDictionary("AppTermination"), GenFnct.GetFromDictionary("Error_ftl"), MessageBoxButton.OK, MessageBoxImage.Error);
            this.Close();
        }


        /// <summary>
        /// Clears the Listview of the Logs
        /// </summary>
        private void LV_Clear()
        {
            LV_Log.ItemsSource = null;
            LV_Log.Items.Clear();
        }


        /// <summary>
        /// Shows the logs of missing files on the UI if "show logs" checkbox is checked
        /// </summary>
        private void Cb_Logs_Action(object sender, RoutedEventArgs e)
        {
            if (sender != null && ((CheckBox)sender).IsChecked == true)
            {
                LV_Log.Visibility = Visibility.Visible;
                if (this.Height <= 650) this.Height = 650;
            }
            else
            {
                LV_Log.Visibility = Visibility.Collapsed;
                if (this.Height >= 650) this.Height = 450;
            }
        }


        /// <summary>
        /// ZIP Folder Generating Function
        /// </summary>
        /// <param name="savePath"> String path of the directory where the created ZIP folder and missing log file will be placed </param>
        /// <param name="pkgNumber"> String representing the PO or Quote number entered by user </param>
        /// <param name="selectedFlt"> List of FileType type objects according to file type checkboxes selected by user </param>
        /// <param name="selectedItems"> List of Item type objects to ZIP </param>
        private void GenerateZipFromPackage(string savePath, string pkgNumber, List<FileType> selectedFlt, List<Item> selectedItems)
        {
            List<string> filesToZip = [];


            bool nsFlag = selectedItems.Any(x => x.Number.Equals("NS", StringComparison.InvariantCultureIgnoreCase));

            foreach (FileType oneFlt in selectedFlt)
            {
                foreach (Item oneItem in selectedItems)
                {
                    if (!oneItem.Number.Equals("NS", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string[] filesFound = [];
                        string filePath = "";
                        foreach (string oneExt in oneFlt.Extension)
                        {
                            // Search for exact file (whether rev is empty or not)
                                filePath = $@"{oneItem.ExpectedFolderLoc(oneFlt.Description)}{oneItem.Number + oneItem.Revision}.{oneExt}";
                            if (File.Exists(filePath)) filesToZip.Add(filePath);
                            else
                            {
                                missingMessages.Add(GenFnct.GetFromDictionary("FileNotFound") + $" {oneFlt.Description}: {oneItem.Number + oneItem.Revision}.{oneExt}");
                                if (Directory.Exists(oneItem.ExpectedFolderLoc(oneFlt.Description)))
                                {
                                    // Search no.2 aimed mainly at STEP files that have many configs (using wildecard after "_" symbol)
                                    filesFound = Directory.GetFiles(oneItem.ExpectedFolderLoc(oneFlt.Description), $"{oneItem.Number + oneItem.Revision}_*.{oneExt}");

                                    if (filesFound.Length > 0) filesToZip.AddRange(filesFound);
                                }
                            }
                        }
                    }
                }
            }

            //foreach (FileType oneFlt in selectedFlt)
            //{
            //    foreach (Item oneItem in selectedItems)
            //    {
            //        if (!oneItem.Number.Equals("NS", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            string[] filesFound = [];
            //            foreach (string oneExt in oneFlt.Extension)
            //            {
            //                // Wild Card search from the getgo
            //                if (Directory.Exists(oneItem.ExpectedFolderLoc(oneFlt.Description)))
            //                {
            //                    filesFound = Directory.GetFiles(oneItem.ExpectedFolderLoc(oneFlt.Description), $"{oneItem.Number + oneItem.Revision}*.{oneExt}");
            //                }

            //                if (filesFound.Length > 0)
            //                {
            //                    filesToZip.AddRange(filesFound);        // File(s) found and added to the list
            //                }
            //                else
            //                {
            //                    missingMessages.Add(GenFnct.GetFromDictionary("FileNotFound") + $" {oneFlt.Description}: {oneItem.Number + oneItem.Revision}.{oneExt}");

            //                    // Recheck without any revision and no Wild card 
            //                    if (!oneItem.Revision.IsNullOrEmpty())
            //                    {
            //                        string filePath = $@"{oneItem.ExpectedFolderLoc(oneFlt.Description)}{oneItem.Number}.{oneExt}";
            //                        if (File.Exists(filePath)) filesToZip.Add(filePath);
            //                        else missingMessages.Add(GenFnct.GetFromDictionary("FileNotFound") + $" {oneFlt.Description}: {oneItem.Number}.{oneExt}");
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            string ZIPPath = savePath + $"\\NITA_{selectedPkg.Name}-{pkgNumber}.zip";
            string logPath = savePath + $"\\NITA_{selectedPkg.Name}-{pkgNumber}-missing.log";

            // Zipping the Files
            if (File.Exists(ZIPPath)) File.Delete(ZIPPath);
            if (filesToZip.Count > 0)
            {
                ZipArchive zip = ZipFile.Open(ZIPPath, ZipArchiveMode.Create);
                foreach (string existingFilePath in filesToZip)
                {
                    zip.CreateEntryFromFile(existingFilePath, Path.GetFileName(existingFilePath), CompressionLevel.Optimal);
                }
                // End ZIP creation process and release resources when loop is over and final ZIP is created
                zip.Dispose();
                MessageBox.Show(GenFnct.GetFromDictionary("ZIPCompleted") + $" {Path.GetFileName(ZIPPath)}.", GenFnct.GetFromDictionary("Success"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else MessageBox.Show(GenFnct.GetFromDictionary("NoFilesZIP") + $" {Path.GetFileName(ZIPPath)}", GenFnct.GetFromDictionary("Info"), MessageBoxButton.OK, MessageBoxImage.Warning);

            // Generating the .log of missing files
            if (File.Exists(logPath)) File.Delete(logPath);
            if (nsFlag || missingMessages.Count > 0)
            {
                TextWriter tWriter = new StreamWriter(logPath);
                if (missingMessages.Count > 0)
                {
                    foreach (string missFilePath in missingMessages) tWriter.WriteLine(missFilePath);
                }
                if (nsFlag) tWriter.WriteLine(GenFnct.GetFromDictionary("NSItems"));
                tWriter.Close();
            }
        }


        /// <summary>
        /// Cleans the list of files to ZIP by sorting out duplicates and keeping only the most recent revisions if multiple instances of a same file exist (especially useful when inlcuding assembly children)
        /// </summary>
        /// <param name="completeList"> "raw" List of Item objects as retrieved by the RetrievePackageDetails() function </param>
        /// <returns> A "clean" list of Item objects that can be taken to the ZIP generating function GenerateZipFromPackage() </returns>
        private List<Item> SortingDuplicates(List<ItemEx> completeList)
        {
            List<Item> finalList = new();

            // Order the items by ascending order (Number wise and alphabetically)
            List<ItemEx> sortedList = completeList.Distinct().OrderBy(o => o.Number + o.Revision).ToList();

            for (int i = 0; i < completeList.Count - 1; i++)
            {
                if (sortedList[i].Number == sortedList[i + 1].Number)
                {
                    int flag = String.Compare(sortedList[i].Revision, sortedList[i + 1].Revision);
                    if (flag == 0 && !finalList.Any(x => x.Number == sortedList[i].Number && x.Revision == sortedList[i].Revision)) // means the revisions are equal => exact duplicate, and checks if the finalList doesnt already contain the same element
                    {
                        finalList.Add(sortedList[i]);
                    }
                    else if (flag < 0 && !finalList.Any(x => x.Number == sortedList[i + 1].Number && x.Revision == sortedList[i + 1].Revision)) // ie. the first string comes before the second string in the alphabetical order : sortedList[i+1] is the most recent rev ("biggest" revision letter)
                    {
                        finalList[^1] = sortedList[i + 1];
                        missingMessages.Add(GenFnct.GetFromDictionary("TwoVersions") + $" {sortedList[i + 1].Number + sortedList[i + 1].Revision} ; {sortedList[i].Number + sortedList[i].Revision}");
                    }
                }
                else
                {
                    finalList.Add(sortedList[i + 1]);
                    if (!finalList.Any(x => x.Number == sortedList[i].Number && x.Revision == sortedList[i].Revision))
                    {
                        finalList.Add(sortedList[i]);
                    }
                }
            }
            return finalList;
        }


        private void btn_language_click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["Lang"] = (string)btn_language.Content;
            Properties.Settings.Default.Save();
            GenFnct.SetLanguage((string)Properties.Settings.Default["Lang"]);
        }
    }
}
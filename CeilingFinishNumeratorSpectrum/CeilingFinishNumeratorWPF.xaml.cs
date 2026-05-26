using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CeilingFinishNumerator
{
    public partial class CeilingFinishNumeratorWPF : Window
    {
        public string CeilingFinishNumberingSelectedName;
        public bool ProcessSelectedLevel;
        public bool SeparatedBySections;
        public bool FillRoomBookParameters;
        public bool ConsiderPhase;
        public ElementId SelectedPhaseId = ElementId.InvalidElementId;
        public ElementId SelectedPhaseFilterId = ElementId.InvalidElementId;

        private List<Level> Levels;
        public Level SelectedLevel = null;

        private List<Parameter> StringParameters;
        public Parameter SelectedParameter = null;

        private readonly List<PhaseSelectionItem> _phaseItems;
        private readonly List<PhaseFilterSelectionItem> _phaseFilterItems;

        private CeilingFinishNumeratorSettings CeilingFinishNumeratorSettingsItem;

        public CeilingFinishNumeratorWPF(List<Parameter> stringParameters, List<Level> levels)
            : this(
                stringParameters,
                levels,
                new List<PhaseSelectionItem>(),
                ElementId.InvalidElementId,
                new List<PhaseFilterSelectionItem>(),
                ElementId.InvalidElementId)
        {
        }

        public CeilingFinishNumeratorWPF(
            List<Parameter> stringParameters,
            List<Level> levels,
            IEnumerable<PhaseSelectionItem> phaseItems,
            ElementId defaultPhaseId,
            IEnumerable<PhaseFilterSelectionItem> phaseFilterItems,
            ElementId defaultPhaseFilterId)
        {
            Levels = levels;
            StringParameters = stringParameters;
            _phaseItems = phaseItems?.ToList() ?? new List<PhaseSelectionItem>();
            _phaseFilterItems = phaseFilterItems?.ToList() ?? new List<PhaseFilterSelectionItem>();
            CeilingFinishNumeratorSettingsItem = CeilingFinishNumeratorSettings.GetSettings();

            InitializeComponent();

            comboBox_LevelSelection.ItemsSource = Levels;
            comboBox_LevelSelection.DisplayMemberPath = "Name";

            comboBox_RoomParameters.ItemsSource = StringParameters;
            comboBox_RoomParameters.DisplayMemberPath = "Definition.Name";

            comboBox_Phase.ItemsSource = _phaseItems;
            comboBox_Phase.DisplayMemberPath = nameof(PhaseSelectionItem.Name);
            comboBox_PhaseFilter.ItemsSource = _phaseFilterItems;
            comboBox_PhaseFilter.DisplayMemberPath = nameof(PhaseFilterSelectionItem.Name);

            var defaultPhaseItem = _phaseItems.FirstOrDefault(p => ElementIdCompat.HasSameValue(p.PhaseId, defaultPhaseId))
                ?? _phaseItems.LastOrDefault();
            comboBox_Phase.SelectedItem = defaultPhaseItem;

            var defaultPhaseFilterItem = _phaseFilterItems.FirstOrDefault(pf => ElementIdCompat.HasSameValue(pf.PhaseFilterId, defaultPhaseFilterId))
                ?? _phaseFilterItems.FirstOrDefault();
            comboBox_PhaseFilter.SelectedItem = defaultPhaseFilterItem;

            // Загрузка сохраненных настроек
            if (CeilingFinishNumeratorSettingsItem != null)
            {
                if (CeilingFinishNumeratorSettingsItem.CeilingFinishNumberingSelectedName == "rbt_EndToEndThroughoutTheProject")
                {
                    rbt_EndToEndThroughoutTheProject.IsChecked = true;
                }
                else if (CeilingFinishNumeratorSettingsItem.CeilingFinishNumberingSelectedName == "rbt_SeparatedByLevels")
                {
                    rbt_SeparatedByLevels.IsChecked = true;
                }

                checkBox_ProcessSelectedLevel.IsChecked = CeilingFinishNumeratorSettingsItem.ProcessSelectedLevel;
                checkBox_SeparatedBySections.IsChecked = CeilingFinishNumeratorSettingsItem.SeparatedBySections;
                checkBox_FillRoomBookParameters.IsChecked = CeilingFinishNumeratorSettingsItem.FillRoomBookParameters;
                checkBox_ConsiderPhase.IsChecked = false;

                if (!string.IsNullOrEmpty(CeilingFinishNumeratorSettingsItem.SelectedLevelName))
                {
                    var selectedLevel = Levels.FirstOrDefault(l => l.Name == CeilingFinishNumeratorSettingsItem.SelectedLevelName);
                    if (selectedLevel != null)
                    {
                        comboBox_LevelSelection.SelectedItem = selectedLevel;
                    }
                    else
                    {
                        if (Levels.Any())
                        {
                            comboBox_LevelSelection.SelectedItem = Levels.First();
                        }
                    }
                }
                else
                {
                    if (Levels.Any())
                    {
                        comboBox_LevelSelection.SelectedItem = Levels.First();
                    }
                }

                if (!string.IsNullOrEmpty(CeilingFinishNumeratorSettingsItem.SelectedParameterName))
                {
                    var selectedParam = StringParameters.FirstOrDefault(p => p.Definition.Name == CeilingFinishNumeratorSettingsItem.SelectedParameterName);
                    if (selectedParam != null)
                    {
                        comboBox_RoomParameters.SelectedItem = selectedParam;
                    }
                    else
                    {
                        // Если параметр не найден, выбираем первый элемент списка, если список не пустой
                        if (StringParameters.Any())
                        {
                            comboBox_RoomParameters.SelectedItem = StringParameters.First();
                        }
                    }
                }
                else
                {
                    // Если SelectedParameterName не задан, выбираем первый элемент списка, если список не пустой
                    if (StringParameters.Any())
                    {
                        comboBox_RoomParameters.SelectedItem = StringParameters.First();
                    }
                }
            }
            else
            {
                if (Levels.Any())
                {
                    comboBox_LevelSelection.SelectedItem = Levels.First();
                }

                if (StringParameters.Any())
                {
                    comboBox_RoomParameters.SelectedItem = StringParameters.First();
                }
            }

            comboBox_RoomParameters.IsEnabled = checkBox_SeparatedBySections.IsChecked == true;
            checkBox_ConsiderPhase.IsEnabled = _phaseItems.Any();

            UpdateControlsState();
            UpdatePhaseSelectorState();
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            if (rbt_EndToEndThroughoutTheProject.IsChecked == true)
            {
                CeilingFinishNumberingSelectedName = rbt_EndToEndThroughoutTheProject.Name;
            }
            else if (rbt_SeparatedByLevels.IsChecked == true)
            {
                CeilingFinishNumberingSelectedName = rbt_SeparatedByLevels.Name;
            }

            ProcessSelectedLevel = checkBox_ProcessSelectedLevel.IsChecked == true;
            SeparatedBySections = checkBox_SeparatedBySections.IsChecked == true;
            FillRoomBookParameters = checkBox_FillRoomBookParameters.IsChecked == true;

            SelectedLevel = comboBox_LevelSelection.SelectedItem as Level;
            SelectedParameter = comboBox_RoomParameters.SelectedItem as Parameter;

            ConsiderPhase = checkBox_ConsiderPhase.IsChecked == true;
            if (ConsiderPhase)
            {
                var selectedPhase = comboBox_Phase.SelectedItem as PhaseSelectionItem;
                var selectedPhaseFilter = comboBox_PhaseFilter.SelectedItem as PhaseFilterSelectionItem;
                if (selectedPhase == null)
                {
                    MessageBox.Show("Выберите стадию для нумерации.", "Нумератор потолка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SelectedPhaseId = selectedPhase.PhaseId;
                SelectedPhaseFilterId = selectedPhaseFilter?.PhaseFilterId ?? ElementId.InvalidElementId;
            }
            else
            {
                SelectedPhaseId = ElementId.InvalidElementId;
                SelectedPhaseFilterId = ElementId.InvalidElementId;
            }

            SaveSettings();

            DialogResult = true;
            Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CeilingFinishNumeratorWPF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                btn_Ok_Click(sender, e);
            }
            else if (e.Key == Key.Escape)
            {
                btn_Cancel_Click(sender, e);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateControlsState();
        }

        private void CheckBox_ProcessSelectedLevel_Checked(object sender, RoutedEventArgs e)
        {
            UpdateControlsState();
        }

        private void UpdateControlsState()
        {
            if (rbt_SeparatedByLevels != null)
            {
                bool isSeparatedByLevels = rbt_SeparatedByLevels.IsChecked == true;
                bool isProcessSelectedLevel = checkBox_ProcessSelectedLevel.IsChecked == true;

                comboBox_LevelSelection.IsEnabled = isSeparatedByLevels && isProcessSelectedLevel;
            }
        }

        private void CheckBox_StateChanged(object sender, RoutedEventArgs e)
        {
            comboBox_RoomParameters.IsEnabled = checkBox_SeparatedBySections.IsChecked == true;
        }

        private void CheckBox_Phase_StateChanged(object sender, RoutedEventArgs e)
        {
            UpdatePhaseSelectorState();
        }

        private void UpdatePhaseSelectorState()
        {
            bool isEnabled = checkBox_ConsiderPhase.IsChecked == true;
            comboBox_Phase.IsEnabled = isEnabled && _phaseItems.Any();
            comboBox_PhaseFilter.IsEnabled = isEnabled && _phaseFilterItems.Any();
        }

        private void SaveSettings()
        {
            CeilingFinishNumeratorSettingsItem = new CeilingFinishNumeratorSettings
            {
                CeilingFinishNumberingSelectedName = CeilingFinishNumberingSelectedName,
                FillRoomBookParameters = FillRoomBookParameters,
                SeparatedBySections = SeparatedBySections,
                SelectedParameterName = SelectedParameter?.Definition.Name,
                ProcessSelectedLevel = ProcessSelectedLevel,
                SelectedLevelName = SelectedLevel?.Name
            };

            CeilingFinishNumeratorSettingsItem.SaveSettings();
        }
    }
}

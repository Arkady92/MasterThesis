using InvertedPendulumTransporterPhysics.Controllers;
using System.Windows;
using System.Windows.Controls;

namespace InvertedPendulumTransporter.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml [Top Menu management]
    /// </summary>
    public partial class MainWindow
    {
        #region Private Members

        #endregion

        #region Public Members

        #endregion

        #region Private Methods
        #region Mode
        /// <summary>
        /// Setup default menu items enablement
        /// </summary>
        private void SetupMenu()
        {
            ShowTargetTrajectoryMenuItem.IsChecked = true;
            ShowCartTrajectoryMenuItem.IsChecked = true;
            ShowPendulumTrajectoryMenuItem.IsChecked = true;

            PIDVoltageMenuItem.IsChecked = true;
            RandomSmoothWindMenuItem.IsChecked = true;
            MediumAccuracyMenuItem.IsChecked = true;

            SolidColorsGraphicsMenuItem.IsChecked = true;
        }

        /// <summary>
        /// Handle load trajectory item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void LoadTrajectoryItem_Click(object sender, RoutedEventArgs e)
        {
            // Load trajectory from file
            ResetButton_Click(null, null);
            var trajectory = trajectoryController.LoadTrajectory();
            if (trajectory == null)
                return;
            SceneControl.UpdateTrajectory(trajectory);

            // Setup system for trajectory mode
            var startPosition = trajectoryController.GetTargetStartPosition();
            XCoordAngle = 0.0;
            YCoordAngle = 0.0;
            systemState.Reset(0.0, 0.0, startPosition.X, startPosition.Y);
            SceneControl.UpdateState(systemState);
            SceneControl.UpdateCamera(systemState);
            if (xCoordVoltageController.ControlType != ControlType.DoublePIDParallel
                && xCoordVoltageController.ControlType != ControlType.DoublePIDCascade
                && xCoordVoltageController.ControlType != ControlType.DoublePDParallel)
                SetMenuVoltage(DoublePDParallelVoltageMenuItem, ControlType.DoublePDParallel);
            UpdateGUI();
        }

        /// <summary>
        /// Handle create trajectory button click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void CreateTrajectoryItem_Click(object sender, RoutedEventArgs e)
        {
            ClearTrajectoryItem_Click(null, null);
            var window = new CreateTrajectoryWindow();
            window.ShowDialog();
            if (window.DialogResult == false || !window.TrajectoryLoaded) return;

            // Load new trajectory
            var fileName = trajectoryController.SaveTrajectory(window.TrajectoryPoints);
            if (fileName == null) return;
            var trajectory = trajectoryController.LoadTrajectory(fileName);
            if (trajectory == null)
                return;
            SceneControl.UpdateTrajectory(trajectory);

            // Setup system for trajectory mode
            var startPosition = trajectoryController.GetTargetStartPosition();
            XCoordAngle = 0.0;
            YCoordAngle = 0.0;
            systemState.Reset(0.0, 0.0, startPosition.X, startPosition.Y);
            SceneControl.UpdateState(systemState);
            SceneControl.UpdateCamera(systemState);
            if (xCoordVoltageController.ControlType != ControlType.DoublePIDParallel
                && xCoordVoltageController.ControlType != ControlType.DoublePIDCascade
                && xCoordVoltageController.ControlType != ControlType.DoublePDParallel
                && xCoordVoltageController.ControlType != ControlType.DoublePDParallel)
                SetMenuVoltage(DoublePDParallelVoltageMenuItem, ControlType.DoublePDParallel);
            UpdateGUI();
        }

        /// <summary>
        /// Handle clear trajectory button click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void ClearTrajectoryItem_Click(object sender, RoutedEventArgs e)
        {
            // Reset system do default state
            ResetButton_Click(null, null);
            SceneControl.ClearTrajectory();
            trajectoryController.Clear();
            systemState.Reset();
            XCoordAngle = 0.0;
            YCoordAngle = 0.0;
            systemState.Reset();
            SceneControl.UpdateState(systemState);
            SceneControl.UpdateCamera(systemState);
            if (xCoordVoltageController.ControlType == ControlType.DoublePIDParallel
                || xCoordVoltageController.ControlType == ControlType.DoublePIDCascade
                || xCoordVoltageController.ControlType == ControlType.DoublePDParallel)
                SetMenuVoltage(PIDVoltageMenuItem, ControlType.PID);
            UpdateGUI();
        }

        /// <summary>
        /// Handle load game button click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void LoadGameItem_Click(object sender, RoutedEventArgs e)
        {
            // Load trajectory
            if (!trajectoryController.TrajectoryEnabled)
                LoadTrajectoryItem_Click(null, null);
            // Setup system for game mode
            ButtonsGrid.Visibility = Visibility.Visible;
            gameController.GameEnabled = true;
            SetMenuVoltage(PIDVoltageMenuItem, ControlType.PID);
        }

        /// <summary>
        /// Handle clear game button click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void ClearGameItem_Click(object sender, RoutedEventArgs e)
        {
            // Reset system to default state
            ButtonsGrid.Visibility = Visibility.Hidden;
            ClearTrajectoryItem_Click(null, null);
            gameController.GameEnabled = false;
        }
        #endregion

        #region Options
        /// <summary>
        /// Setup current wind tipe visual
        /// </summary>
        /// <param name="sender">Appropriate menu item</param>
        /// <param name="windType">Type of wind</param>
        private void SetMenuWind(MenuItem sender, WindType windType)
        {
            foreach (var item in WindMenuItem.Items)
                (item as MenuItem).IsChecked = false;
            sender.IsChecked = true;
            windController.WindType = windType;
        }

        /// <summary>
        /// Handle random peak wind menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void RandomPeakWindMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuWind(sender as MenuItem, WindType.RandomPeak);
        }

        /// <summary>
        /// Handle random switch wind menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void RandomSwitchWindMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuWind(sender as MenuItem, WindType.RandomSwitch);
        }

        /// <summary>
        /// Handle random smooth wind menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void RandomSmoothWindMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuWind(sender as MenuItem, WindType.RandomSmooth);
        }

        /// <summary>
        /// Setup current trajectory accuracy visual
        /// </summary>
        /// <param name="sender">Appropriate menu item</param>
        /// <param name="accuracy">Trajectory accuracy type</param>
        private void SetTrajectoryAccuracy(MenuItem sender, AccuracyType accuracy)
        {
            foreach (var item in TrajectoryAccuracyMenuItem.Items)
                (item as MenuItem).IsChecked = false;
            sender.IsChecked = true;
            trajectoryController.SetAccuracy(accuracy);
        }

        /// <summary>
        /// Handle show target trajectory menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void ShowTargetTrajectoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            SceneControl.ShowTargetTrajectory(menuItem.IsChecked);
        }

        /// <summary>
        /// Handle show cart trajectory menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void ShowCartTrajectoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            SceneControl.ShowCartTrajectory(menuItem.IsChecked);
        }

        /// <summary>
        /// Handle show pendulum trajectory menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void ShowPendulumTrajectoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            SceneControl.ShowPendulumTrajectory(menuItem.IsChecked);
        }

        /// <summary>
        /// Setup current voltage controller visual
        /// </summary>
        /// <param name="sender">Appropriate menu item</param>
        /// <param name="controlType">Wind control type</param>
        private void SetMenuVoltage(MenuItem sender, ControlType controlType)
        {
            foreach (var item in VoltageMenuItem.Items)
                (item as MenuItem).IsChecked = false;
            sender.IsChecked = true;
            xCoordVoltageController.ControlType = controlType;
            yCoordVoltageController.ControlType = controlType;
        }

        /// <summary>
        /// Handle double PID parallel controller menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void DoublePIDParallelVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.DoublePIDParallel);
        }

        /// <summary>
        /// Handle double PD parallel controller menu item click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoublePDParallelVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.DoublePDParallel);
        }

        /// <summary>
        /// Handle double PID cascade controller menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void DoublePIDCascadeVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.DoublePIDCascade);
        }

        /// <summary>
        /// Handle PID controller menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void PIDVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.PID);
        }

        /// <summary>
        /// Handle sinusoidal controller menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void SinusoidalVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.Sinusoidal);
        }

        /// <summary>
        /// Handle random controller menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void RandomVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.Random);
        }

        /// <summary>
        /// Handle none controller menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void NoneVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.None);
        }

        /// <summary>
        /// Handle trajectory ultra accuracy menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void UltraAccuracyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetTrajectoryAccuracy(sender as MenuItem, AccuracyType.Ultra);
        }

        /// <summary>
        /// Handle trajectory high accuracy menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void HighAccuracyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetTrajectoryAccuracy(sender as MenuItem, AccuracyType.High);
        }

        /// <summary>
        /// Handle trajectory medium accuracy menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void MediumAccuracyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetTrajectoryAccuracy(sender as MenuItem, AccuracyType.Medium);
        }

        /// <summary>
        /// Handle trajectory low accuracy menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void LowAccuracyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetTrajectoryAccuracy(sender as MenuItem, AccuracyType.Low);
        }

        /// <summary>
        /// Handle high level textures menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void TexturesGraphicsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SceneControl.SetupHighLevelGraphics();
            SolidColorsGraphicsMenuItem.IsChecked = false;
        }

        /// <summary>
        /// Handle low level textures menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void SolidColorsGraphicsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SceneControl.SetupLowLevelGraphics();
            TexturesGraphicsMenuItem.IsChecked = false;
        }
        #endregion

        #region About
        /// <summary>
        /// Handle application info menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void ApplicationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new AboutWindow();
            window.SetupWindowType(WindowType.Application);
            window.ShowDialog();
        }

        /// <summary>
        /// Handle system mechanics menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void SystemMechanicsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new AboutWindow();
            window.SetupWindowType(WindowType.SystemMechanics);
            window.ShowDialog();
        }

        /// <summary>
        /// Handle authors info menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void AuthorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new AboutWindow();
            window.SetupWindowType(WindowType.Author);
            window.ShowDialog();
        }

        /// <summary>
        /// Setup visibility for window elements
        /// </summary>
        /// <param name="visibility">Type of visibility</param>
        private void SetElementsVisibility(Visibility visibility)
        {
            Menu.Visibility = visibility;
            AnimationControlPanel.Visibility = visibility;
            SystemParametersPanel.Visibility = visibility;
            WindParametersPanel.Visibility = visibility;
            SystemStateInfo.Visibility = visibility;
            SceneControl.Visibility = visibility;
            PlotsControl.Visibility = visibility;
            ControlPanelBorder.Visibility = visibility;
        }

        /// <summary>
        /// Handle help menu item click
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var count = 9;
            SetElementsVisibility(Visibility.Hidden);

            // Show help as a series of windows
            for (int i = 0; i < count; i++)
            {
                var window = new AboutWindow();
                switch (i)
                {
                    case 0:
                        window.SetupWindowType(WindowType.HelpMenuMode);
                        Menu.Visibility = Visibility.Visible;
                        break;
                    case 1:
                        window.SetupWindowType(WindowType.HelpMenuOptions);
                        break;
                    case 2:
                        window.SetupWindowType(WindowType.HelpMenuAbout);
                        break;
                    case 3:
                        window.SetupWindowType(WindowType.HelpAnimationPanel);
                        AnimationControlPanel.Visibility = Visibility.Visible;
                        break;
                    case 4:
                        window.SetupWindowType(WindowType.HelpSystemParameters);
                        SystemParametersPanel.Visibility = Visibility.Visible;
                        break;
                    case 5:
                        window.SetupWindowType(WindowType.HelpWindParameters);
                        WindParametersPanel.Visibility = Visibility.Visible;
                        break;
                    case 6:
                        window.SetupWindowType(WindowType.HelpSystemStateInfo);
                        SystemStateInfo.Visibility = Visibility.Visible;
                        break;
                    case 7:
                        window.SetupWindowType(WindowType.HelpSimulationScene);
                        SceneControl.Visibility = Visibility.Visible;
                        ControlPanelBorder.Visibility = Visibility.Visible;
                        break;
                    case 8:
                        window.SetupWindowType(WindowType.HelpPlots);
                        PlotsControl.Visibility = Visibility.Visible;
                        break;
                }
                window.SetupHelpWindow(i == count - 1);
                window.ShowDialog();
                if (window.DialogResult == false)
                    break;
            }
            SetElementsVisibility(Visibility.Visible);
        }
        #endregion
        #endregion

        #region Public Methods

        #endregion
    }
}
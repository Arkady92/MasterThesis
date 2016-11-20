using InvertedPendulumTransporterPhysics.Controllers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace InvertedPendulumTransporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Mode
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

        private void LoadTrajectoryItem_Click(object sender, RoutedEventArgs e)
        {
            ResetButton_Click(null, null);
            var trajectory = trajectoryController.LoadTrajectory();
            if (trajectory == null)
                return;
            SceneControl.UpdateTrajectory(trajectory);

            var startPosition = trajectoryController.GetTargetStartPosition();
            XCoordAngle = 0.0;
            YCoordAngle = 0.0;
            systemState.Reset(0.0, 0.0, startPosition.X, startPosition.Y);
            SceneControl.UpdateState(systemState);
            SceneControl.UpdateCamera(systemState);
            if (xCoordVoltageController.ControlType != ControlType.DoublePIDParallel
                && xCoordVoltageController.ControlType != ControlType.DoublePIDCascade)
                SetMenuVoltage(DoublePIDParallelVoltageMenuItem, ControlType.DoublePIDParallel);
            UpdateGUI();
        }


        private void CreateTrajectoryItem_Click(object sender, RoutedEventArgs e)
        {
            ClearTrajectoryItem_Click(null, null);
            var window = new CreateTrajectoryWindow();
            window.ShowDialog();
            if (window.DialogResult == false || !window.TrajectoryLoaded) return;

            var fileName = trajectoryController.SaveTrajectory(window.TrajectoryPoints);
            if (fileName == null) return;
            var trajectory = trajectoryController.LoadTrajectory(fileName);
            if (trajectory == null)
                return;
            SceneControl.UpdateTrajectory(trajectory);

            var startPosition = trajectoryController.GetTargetStartPosition();
            XCoordAngle = 0.0;
            YCoordAngle = 0.0;
            systemState.Reset(0.0, 0.0, startPosition.X, startPosition.Y);
            SceneControl.UpdateState(systemState);
            SceneControl.UpdateCamera(systemState);
            if (xCoordVoltageController.ControlType != ControlType.DoublePIDParallel
                && xCoordVoltageController.ControlType != ControlType.DoublePIDCascade)
                SetMenuVoltage(DoublePIDParallelVoltageMenuItem, ControlType.DoublePIDParallel);
            UpdateGUI();
        }

        private void ClearTrajectoryItem_Click(object sender, RoutedEventArgs e)
        {
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
                || xCoordVoltageController.ControlType == ControlType.DoublePIDCascade)
                SetMenuVoltage(PIDVoltageMenuItem, ControlType.PID);
            UpdateGUI();
        }

        private void LoadGameItem_Click(object sender, RoutedEventArgs e)
        {
            if (!trajectoryController.TrajectoryEnabled)
                LoadTrajectoryItem_Click(null, null);
            ButtonsGrid.Visibility = Visibility.Visible;
            gameController.GameEnabled = true;
            SetMenuVoltage(NoneVoltageMenuItem, ControlType.PID);
        }

        private void ClearGameItem_Click(object sender, RoutedEventArgs e)
        {
            ButtonsGrid.Visibility = Visibility.Hidden;
            ClearTrajectoryItem_Click(null, null);
            gameController.GameEnabled = false;
        }
        #endregion

        #region Options
        private void SetMenuWind(MenuItem sender, WindType windType)
        {
            foreach (var item in WindMenuItem.Items)
                (item as MenuItem).IsChecked = false;
            sender.IsChecked = true;
            windController.WindType = windType;
        }

        private void RandomPeakWindMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuWind(sender as MenuItem, WindType.RandomPeak);
        }

        private void RandomSwitchWindMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuWind(sender as MenuItem, WindType.RandomSwitch);
        }

        private void RandomSmoothWindMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuWind(sender as MenuItem, WindType.RandomSmooth);
        }

        private void SetTrajectoryAccuracy(MenuItem sender, AccuracyType accuracy)
        {
            foreach (var item in TrajectoryAccuracyMenuItem.Items)
                (item as MenuItem).IsChecked = false;
            sender.IsChecked = true;
            trajectoryController.SetAccuracy(accuracy);
        }

        private void ShowTargetTrajectoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            SceneControl.ShowTargetTrajectory(menuItem.IsChecked);
        }

        private void ShowCartTrajectoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            SceneControl.ShowCartTrajectory(menuItem.IsChecked);
        }

        private void ShowPendulumTrajectoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            SceneControl.ShowPendulumTrajectory(menuItem.IsChecked);
        }

        private void SetMenuVoltage(MenuItem sender, ControlType controlType)
        {
            foreach (var item in VoltageMenuItem.Items)
                (item as MenuItem).IsChecked = false;
            sender.IsChecked = true;
            xCoordVoltageController.ControlType = controlType;
            yCoordVoltageController.ControlType = controlType;
        }

        private void DoublePIDParallelVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.DoublePIDParallel);
        }

        private void DoublePIDCascadeVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.DoublePIDCascade);
        }

        private void PIDVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.PID);
        }

        private void SinusoidalVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.Sinusoidal);
        }

        private void RandomVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.Random);
        }

        private void NoneVoltageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuVoltage(sender as MenuItem, ControlType.None);
        }

        private void UltraAccuracyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetTrajectoryAccuracy(sender as MenuItem, AccuracyType.Ultra);
        }

        private void HighAccuracyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetTrajectoryAccuracy(sender as MenuItem, AccuracyType.High);
        }

        private void MediumAccuracyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetTrajectoryAccuracy(sender as MenuItem, AccuracyType.Medium);
        }

        private void LowAccuracyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetTrajectoryAccuracy(sender as MenuItem, AccuracyType.Low);
        }

        private void TexturesGraphicsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SceneControl.SetupHighGradeTextures();
            SolidColorsGraphicsMenuItem.IsChecked = false;
        }

        private void SolidColorsGraphicsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SceneControl.SetupLowGradeTextures();
            TexturesGraphicsMenuItem.IsChecked = false;
        }
        #endregion

        #region About
        private void ApplicationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new AboutWindow();
            window.SetupWindowType(WindowType.Application);
            window.ShowDialog();
        }

        private void SystemMechanicsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new AboutWindow();
            window.SetupWindowType(WindowType.SystemMechanics);
            window.ShowDialog();
        }

        private void AuthorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new AboutWindow();
            window.SetupWindowType(WindowType.Author);
            window.ShowDialog();
        }

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

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var count = 9;
            SetElementsVisibility(Visibility.Hidden);
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
    }
}
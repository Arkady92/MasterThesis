using InvertedPendulumTransporterPhysics.Controllers;
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
            gameController.GameEnabled = true;
            SetMenuVoltage(NoneVoltageMenuItem, ControlType.PID);
        }

        private void ClearGameItem_Click(object sender, RoutedEventArgs e)
        {
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

        #endregion
    }
}
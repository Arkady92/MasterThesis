using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace InvertedPendulumTransporter.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        private Dictionary<WindowType, string> windowsTitles;
        private Dictionary<WindowType, string> windowsContents;
        private Dictionary<WindowType, TextBlock> windowsTextBlocks;
        private WindowType windowType;

        public AboutWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            windowsTitles = new Dictionary<WindowType, string>();
            windowsTitles.Add(WindowType.Application, "Application");
            windowsTitles.Add(WindowType.SystemMechanics, "System Mechanics ");
            windowsTitles.Add(WindowType.Author, "Author");
            windowsTitles.Add(WindowType.HelpAnimationPanel, "Animation Panel");
            windowsTitles.Add(WindowType.HelpMenuMode, "Menu");
            windowsTitles.Add(WindowType.HelpMenuOptions, "Menu");
            windowsTitles.Add(WindowType.HelpMenuAbout, "Menu");
            windowsTitles.Add(WindowType.HelpPlots, "Plots");
            windowsTitles.Add(WindowType.HelpSimulationScene, "Simulation Scene");
            windowsTitles.Add(WindowType.HelpSystemParameters, "System Parameters");
            windowsTitles.Add(WindowType.HelpSystemStateInfo, "System State");
            windowsTitles.Add(WindowType.HelpWindParameters, "Wind Parameters");
            windowsContents = new Dictionary<WindowType, string>();
            windowsContents.Add(WindowType.Application, "Simulation of the transporter for an inverted pendulum on a cart.\n"
                + "\nThe system allows user to:\n"
                + "+ Study the behavior using a wide range of parameters.\n"
                + "+ Stabilize the pendulum through various controllers.\n"
                + "+ Apply noises in form of the wind force.\n"
                + "+ Setup a trajectory followed by the system.\n"
                + "+ Play a simple game using a manual control of the movement of the pendulum."
                );
            windowsContents.Add(WindowType.SystemMechanics, "The project is based on well-known problem of two-dimensional system consisting of an inverted pendulum mounted on a movable platform. "
                + "The main task of the system is to keep the pendulum in an unstable equilibrium and respond to noises from the environment through the special voltage controller of the platform's engine."
                + "In the project the system is transferred to a three-dimensional world in which two independent systems associated with the horizontal directions of the principal axes are integrated into a unit. "
                + "As a result, the movement trajectory can be applied to the system and the pendulum should be transported according to given trajectory. " 
                + "An additional element of the project is adding the wind force. " 
                + "The transporter have to deal with the noise in such a way as to minimize the risk of losing control of the pendulum.");
            windowsContents.Add(WindowType.Author, "Jakub Abelski (Arkady)\n"
                + "\nWarsaw University of Technology\n"
                + "Faculty of Mathematics and Information Science");
            windowsContents.Add(WindowType.HelpAnimationPanel,
                "You can control the simulation's progress via the animation's management panel.\n  [Play] button starts and resume the simulation\n  [Pause] button stops the visualization's progress at given point.\n  [Reset] button clear the animation and setup the environment to the default configuration.");
            windowsContents.Add(WindowType.HelpMenuMode,
                "Application's menu is divided into three sections:\n  [Mode] - simulation's type change.\n\nYou can choose between three application's modules. The default one is the pendulum stabilization. If you want to switch to the trajectory mode select [Load Trajectory] to load one of predefine trajectories or [Create Trajectory] to create a new one by specifying the parametrization. Whether you want to try the game mode select [Load Game]. In this mode you are able to cotrol the pendulum manually (remember to press [Play]). \nBy clicking [Clear] buttons in both situations the application changes mode to the default.");
            windowsContents.Add(WindowType.HelpMenuOptions,
                "Application's menu is divided into three sections:\n  [Options] - simulation's options change. \n\nYou can customize some of the system's settings:\n  [Controller Type] - for simple stabilization choose single controllers for trajectory movement the double one.\n  [Wind Generator] - specify how the wind force should be generated.\n  [Trajectory Enablement] - choose whether to show the system trajectories.\n  [Trajectory Accuracy] - set how accurate the tracking algorithm should be.\n  [Graphics] - choose between two visualization types.");
            windowsContents.Add(WindowType.HelpMenuAbout,
                "Application's menu is divided into three sections:\n  [About] - information about program. \n\nIf you are looking for some information about the application, system's mechanics or author click one of the given tabs.");
            windowsContents.Add(WindowType.HelpPlots,
                "During the animation's progress some usefull data like controll's error or engine's voltage are collected in special structures to have the possibility of presenting the results in form of plots. One plot holds X and Y coordinates values. \nBy clicking on the given line you can see the accurate values. \nIf you want to save the plot as image right click outside the plot's area.");
            windowsContents.Add(WindowType.HelpSimulationScene,
                "This window is responsible for showing the system's visualization. The transporter is represented as a platform with four wheels and an inverted pendulum attached at the top. The simulation area is limited, so that you cannot fall from the drawn surface. To facilitate movement tracking there is a grid added on the surface.\n\nYou can manipulate the camera on different ways:\n  [RMB + Move] - rotate the camera around the transporter.\n  [MMB + Move] - move the camera to target position (in play mode the camera target is fixed on the transporter).\n  [Mouse Wheel] - zoom in or out the camera. \n  [Click on the rotation box (right bottom corner)] - rotate the camera to the main axis.");
            windowsContents.Add(WindowType.HelpSystemParameters,
                "Here are some system's parameters which can be modified before the simulation's start. For example you can customize the pendulum's length and mass, change the initial angles or adjust the time's delta for faster animation. When you click the [Reset] button all changes will be reverted. \n\nRemember that the system is not able to stabilize in all situations. When you select the set of parameters that will be impossible to stabilize then you will be prompted that you lost the controllability of the system.");
            windowsContents.Add(WindowType.HelpSystemStateInfo,
                "All information about current system's state is presented in this panel. \n\nInteresting fact is that the full system's state is calculated in each simulation's frame by solving the state-space equation using Runge-Kutta algorithm.");
            windowsContents.Add(WindowType.HelpWindParameters,
                "During the animation's progress you are able to modify the wind conditions. You can set the wind power and how fast the wind force direction will be changed.");
            windowsTextBlocks = new Dictionary<WindowType, TextBlock>();
            windowsTextBlocks.Add(WindowType.Application, ApplicationContentTextBlock);
            windowsTextBlocks.Add(WindowType.SystemMechanics, SystemMechanicsContentTextBlock);
            windowsTextBlocks.Add(WindowType.Author, AuthorContentTextBlock);
            windowsTextBlocks.Add(WindowType.HelpAnimationPanel, HelpContentTextBlock);
            windowsTextBlocks.Add(WindowType.HelpMenuMode, HelpContentTextBlock);
            windowsTextBlocks.Add(WindowType.HelpMenuOptions, HelpContentTextBlock);
            windowsTextBlocks.Add(WindowType.HelpMenuAbout, HelpContentTextBlock);
            windowsTextBlocks.Add(WindowType.HelpPlots, HelpContentTextBlock);
            windowsTextBlocks.Add(WindowType.HelpSimulationScene, HelpContentTextBlock);
            windowsTextBlocks.Add(WindowType.HelpSystemParameters, HelpContentTextBlock);
            windowsTextBlocks.Add(WindowType.HelpSystemStateInfo, HelpContentTextBlock);
            windowsTextBlocks.Add(WindowType.HelpWindParameters, HelpContentTextBlock);
        }

        public void SetupWindowType(WindowType type)
        {
            windowType = type;
            HelpProgressButton.Visibility = Visibility.Hidden;
            TitleTextBlock.Text = windowsTitles[type];
            windowsTextBlocks[type].Text = windowsContents[type];
            windowsTextBlocks[type].Visibility = Visibility.Visible;
        }

        public void SetupHelpWindow(bool finish)
        {
            HelpProgressButton.Visibility = Visibility.Visible;
            HelpProgressButton.Content = finish ? "Finish" : "Next";
        }

        private void HelpProgressButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }

    public enum WindowType
    {
        Application,
        SystemMechanics,
        Author,
        HelpMenuMode,
        HelpMenuOptions,
        HelpMenuAbout,
        HelpAnimationPanel,
        HelpSystemParameters,
        HelpWindParameters,
        HelpSystemStateInfo,
        HelpSimulationScene,
        HelpPlots
    }
}

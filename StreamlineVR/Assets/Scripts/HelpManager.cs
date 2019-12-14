using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{
  public GameObject mainP;
  public GameObject helpP;
  public Text detailsBox;
  public GameObject mainButtons;
  public GameObject toolButtons;

  private void OnEnable()
  {
    helpP.SetActive(false);
  }

  public void StartHelp()
  {
    mainP.SetActive(false);
    helpP.SetActive(true);
    toolButtons.SetActive(false);
  }

  public void EndHelp()
  {
    mainP.SetActive(true);
    helpP.SetActive(false);
  }

  public void GoTools()
  {
    toolButtons.SetActive(true);
    mainButtons.SetActive(false);
    detailsBox.text = "Select a tool to learn about it";
  }

  public void NoTools()
  {
    toolButtons.SetActive(false);
    mainButtons.SetActive(true);
    detailsBox.text = "Select one of the items on the left to see details about them";
  }

  public void Interaction()
  {
    detailsBox.text =

            "Menu Interaction:\n" +
            "To interact with the menus move your left controller into a button and press the trigger to press it.\n" +
            "The trigger on the controller will glow and vibrate when you are able to press the button.\n\n" +
            "Hand Menu Toggle:\n" +
            "Toggle the menu attached to your right controller, press the button above the touchpad on the right controller.\n\n" +
            "Laser Toggle:\n" +
            "Each of the tools has an option to use the laser pointer represented as a checkbox.\n" +
            "Checking this box will have the laser appear and can be used with the tools\n" +
            "Without this checked it will use the selection from the selection tool";

  }

  public void Movement()
  {
    detailsBox.text =
            "Movement: \n" +
            "Press down on the left touchpad to begin moving.\n" +
            "You will move in the direction the left controller is pointing.\n" +
            "Moving your thumb up or down from the center will control you movement speed.\n" +
            "This will also allow yout to move backwards.\n" +
            "Pulling the left trigger while moving will make you move faster.";
  }

  public void HUD()
  {
    detailsBox.text =
            "HUD Camera Views:\n" +
            "The HUD displays some view of the model to assist in navigation.\n" +
            "There are 4 views: Top, Main, Front, and Side.\n" +
            "The green sphere is the players current position.\n\n" +
            "HUD Repositioning:\n" +
            "To Move the HUD grab the sphere on the bottom of the panel with the trigger on either controller.\n" +
            "While keeping the trigger down the panel will stay attached to the hand until the trigger is released.\n" +
            "The HUD will now stay in that position." +
            "HUD Toggle:\n" +
            "The HUD can also be hidden but is toggled by pressing the button above the touchpad on the left controller.";
  }

  public void Transparency()
  {
    detailsBox.text =
            "Select Object:\n" +
            "Pressig the trigger on the right controller while pointing the laser at an object will select it.\n" +
            "The selected object may also come from the selection tool.\n\n" +
            "Changing the Transparency:\n" +
            "Once an object is selected that object's transparency can be changed by grabbing and moving the slider.";
  }

  public void Waypoints()
  {
    detailsBox.text =
      "Create Waypoint:\n" +
      "With the laser, press the trigger on the right controller will create a waypoint where the laser hits another object.\n" +
      "Without the laser, pressing the \"New\" button will create a waypoint at your current position.\n\n" +
      "Delete Waypoint:\n" +
      "With laser, press the trigger on the right controller while pointing at a waypoint will delete it.\n" +
      "Without the laser, select the waypoint from the list of waypoints.\n" +
      "Once one or more is selected pressing the \"Delete\" button will delete the waypoints." +
      "The drop down at the top of the waypoint panel determines what the laser will do.\n";
  }

  public void Selection()
  {
    detailsBox.text = 
      "Selecting an Object:\n" +
      "Press the trigger on the right controller while the laser is pointed at an object to select it.\n\n" +
      "Displays:\n" +
      "On the top of the panel you will see the currently selected object's name.";
  }

  public void Settings()
  {
    detailsBox.text =
            "Advanced Movement:\n" +
            "This will toggle the advanced movement option.\n" +
            "The advanced movement allows the touchpad to be fully used to control your movement.\n\n" +
            "Save:\n" +
            "This will save the current models into the filename that was specified in the setup.\n\n" +
            "ToolTips:\n" +
            "This toggles test over the left controller that displays the funnctionality of buttons in the menu.\n\n" +
            "Exit:\n" +
            "This will return you back to the desktop setup,";
  }

  public void Playback()
  {
    detailsBox.text =
            "Playback Controls:\n" +
            "Along the bottom of the panel there are a row of standard plaback buttons.\n" +
            "From left to right: Skip to last, Step back one, Play backward, Play forward, Step forward one, Skip to last, and Loop.\n" +
            "The play buttons will become pause buttons when it is playing.\n\n" +
            "Playback Speed:\n" +
            "There are two buttons on the top right corner of the screen that control the delay between frames.\n" +
            "The button on the right increases the delay and the left decreases.\n\n" +
            "Progress:\n" +
            "The bar in the center visually represents how far along in the playback you are.\n" +
            "There is also a numerical representation at the top as a fraction.\n" +
            "To the right of the fraction is the current timestep's name";
  }

  public void Color()
  {
    detailsBox.text =
        "Selected Color:\n" +
        "There is a circle on the menu that is the current selected color.\n" +
        "This color determines the color of the selected object.\n\n" +
        "Applying Color:\n" +
        "Pressing the trigger on the right controller while it is pointed at an object will color it with the selected color.\n" +
        "This can be done while the menu is hidden.";
  }
}

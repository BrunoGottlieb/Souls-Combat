using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class InputManager : MonoBehaviour
{
    public static bool PS4Inputs;
    // Jump
    public static KeyCode dodgeKeyboard = KeyCode.Space;
    public static KeyCode dodgeJoystick = KeyCode.Joystick1Button0;

    // Heal
    public static KeyCode estusKeyboard = KeyCode.R;
    public static KeyCode estusJoystick = KeyCode.Joystick1Button2;

    // Primary Attack
    public static KeyCode primaryKeyboard = KeyCode.Mouse1;
    public static KeyCode primaryJoystick = KeyCode.Joystick1Button4;

    // Secondary Attack
    public static KeyCode secondaryKeyboard = KeyCode.Mouse0;
    public static KeyCode secondaryJoystick = KeyCode.Joystick1Button5;

    // Draw Sword
    public static KeyCode drawKeyboard = KeyCode.Mouse2;
    public static KeyCode drawJoystick = KeyCode.Joystick1Button3;

    // Change camera
    public static KeyCode cameraKeyboard = KeyCode.C;
    public static KeyCode cameraJoystick = KeyCode.Joystick1Button9;

    // Restart
    public static KeyCode restartKeyboard = KeyCode.R;
    public static KeyCode restartJoystick = KeyCode.Joystick1Button6;
    public static KeyCode restartPS4 = KeyCode.Joystick1Button13;

    // Pause
    public static KeyCode pauseKeyboard = KeyCode.Escape;
    public static KeyCode pauseJoystick = KeyCode.Joystick1Button7;
    public static KeyCode pausePS4 = KeyCode.Joystick1Button8;

    private static bool triggerPressed = false;

    private void Update()
    {
        if (Input.GetAxisRaw("JoystickTrigger") > -0.1f && Input.GetAxisRaw("JoystickTrigger") < 0.1f) triggerPressed = false;
        else triggerPressed = true;

        print("Pressed: " + triggerPressed);

        print(Input.GetAxisRaw("JoystickTrigger"));
    }

    public static bool GetDodgeInput()
    {
        return Input.GetKeyDown(dodgeKeyboard) || Input.GetKeyDown(dodgeJoystick);
    }

    public static bool GetEstusInput()
    {
        return Input.GetKeyDown(estusKeyboard) || Input.GetKeyDown(estusJoystick);
    }

    public static bool GetPrimaryAttackInput()
    {
        return Input.GetKeyDown(primaryKeyboard) || Input.GetKeyDown(primaryJoystick) || (Input.GetAxisRaw("JoystickTrigger") < 0 && !triggerPressed);
    }

    public static bool GetSecondaryAttackInput()
    {
        return Input.GetKeyDown(secondaryKeyboard) || Input.GetKeyDown(secondaryJoystick) || (Input.GetAxisRaw("JoystickTrigger") > 0 && !triggerPressed);
    }

    public static bool GetDrawSwordInput()
    {
        return Input.GetKeyDown(drawKeyboard) || Input.GetKeyDown(drawJoystick);
    }

    public static bool GetCameraInput()
    {
        return Input.GetKeyDown(cameraKeyboard) || Input.GetKeyDown(cameraJoystick);
    }

    public static bool GetRestartInput()
    {
        if (PS4Inputs)
        {
            return Input.GetKeyDown(restartKeyboard) || Input.GetKeyDown(restartPS4);
        }
        return Input.GetKeyDown(restartKeyboard) || Input.GetKeyDown(restartJoystick);
    }

    public static bool GetPauseInput()
    {
        if (PS4Inputs)
        {
            return Input.GetKeyDown(pauseKeyboard) || Input.GetKeyDown(pausePS4);
        }
        return Input.GetKeyDown(pauseKeyboard) || Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(pauseJoystick);
    }

}

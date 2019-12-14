//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Valve.VR
{
    using System;
    using UnityEngine;
    
    
    public partial class SteamVR_Actions
    {
        
        private static SteamVR_Action_Boolean p_default_InteractUI;
        
        private static SteamVR_Action_Boolean p_default_Teleport;
        
        private static SteamVR_Action_Boolean p_default_GrabPinch;
        
        private static SteamVR_Action_Boolean p_default_GrabGrip;
        
        private static SteamVR_Action_Pose p_default_Pose;
        
        private static SteamVR_Action_Skeleton p_default_SkeletonLeftHand;
        
        private static SteamVR_Action_Skeleton p_default_SkeletonRightHand;
        
        private static SteamVR_Action_Single p_default_Squeeze;
        
        private static SteamVR_Action_Boolean p_default_HeadsetOnHead;
        
        private static SteamVR_Action_Single p_default_speed;
        
        private static SteamVR_Action_Boolean p_default_move;
        
        private static SteamVR_Action_Boolean p_default_toggle_menu;
        
        private static SteamVR_Action_Vector2 p_default_touchpadact;
        
        private static SteamVR_Action_Boolean p_default_LockRotation;
        
        private static SteamVR_Action_Boolean p_default_Tgrab;
        
        private static SteamVR_Action_Boolean p_default_GoScale;
        
        private static SteamVR_Action_Vibration p_default_Haptic;
        
        public static SteamVR_Action_Boolean default_InteractUI
        {
            get
            {
                return SteamVR_Actions.p_default_InteractUI.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean default_Teleport
        {
            get
            {
                return SteamVR_Actions.p_default_Teleport.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean default_GrabPinch
        {
            get
            {
                return SteamVR_Actions.p_default_GrabPinch.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean default_GrabGrip
        {
            get
            {
                return SteamVR_Actions.p_default_GrabGrip.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Pose default_Pose
        {
            get
            {
                return SteamVR_Actions.p_default_Pose.GetCopy<SteamVR_Action_Pose>();
            }
        }
        
        public static SteamVR_Action_Skeleton default_SkeletonLeftHand
        {
            get
            {
                return SteamVR_Actions.p_default_SkeletonLeftHand.GetCopy<SteamVR_Action_Skeleton>();
            }
        }
        
        public static SteamVR_Action_Skeleton default_SkeletonRightHand
        {
            get
            {
                return SteamVR_Actions.p_default_SkeletonRightHand.GetCopy<SteamVR_Action_Skeleton>();
            }
        }
        
        public static SteamVR_Action_Single default_Squeeze
        {
            get
            {
                return SteamVR_Actions.p_default_Squeeze.GetCopy<SteamVR_Action_Single>();
            }
        }
        
        public static SteamVR_Action_Boolean default_HeadsetOnHead
        {
            get
            {
                return SteamVR_Actions.p_default_HeadsetOnHead.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Single default_speed
        {
            get
            {
                return SteamVR_Actions.p_default_speed.GetCopy<SteamVR_Action_Single>();
            }
        }
        
        public static SteamVR_Action_Boolean default_move
        {
            get
            {
                return SteamVR_Actions.p_default_move.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean default_toggle_menu
        {
            get
            {
                return SteamVR_Actions.p_default_toggle_menu.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Vector2 default_touchpadact
        {
            get
            {
                return SteamVR_Actions.p_default_touchpadact.GetCopy<SteamVR_Action_Vector2>();
            }
        }
        
        public static SteamVR_Action_Boolean default_LockRotation
        {
            get
            {
                return SteamVR_Actions.p_default_LockRotation.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean default_Tgrab
        {
            get
            {
                return SteamVR_Actions.p_default_Tgrab.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean default_GoScale
        {
            get
            {
                return SteamVR_Actions.p_default_GoScale.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Vibration default_Haptic
        {
            get
            {
                return SteamVR_Actions.p_default_Haptic.GetCopy<SteamVR_Action_Vibration>();
            }
        }
        
        private static void InitializeActionArrays()
        {
            Valve.VR.SteamVR_Input.actions = new Valve.VR.SteamVR_Action[] {
                    SteamVR_Actions.default_InteractUI,
                    SteamVR_Actions.default_Teleport,
                    SteamVR_Actions.default_GrabPinch,
                    SteamVR_Actions.default_GrabGrip,
                    SteamVR_Actions.default_Pose,
                    SteamVR_Actions.default_SkeletonLeftHand,
                    SteamVR_Actions.default_SkeletonRightHand,
                    SteamVR_Actions.default_Squeeze,
                    SteamVR_Actions.default_HeadsetOnHead,
                    SteamVR_Actions.default_speed,
                    SteamVR_Actions.default_move,
                    SteamVR_Actions.default_toggle_menu,
                    SteamVR_Actions.default_touchpadact,
                    SteamVR_Actions.default_LockRotation,
                    SteamVR_Actions.default_Tgrab,
                    SteamVR_Actions.default_GoScale,
                    SteamVR_Actions.default_Haptic};
            Valve.VR.SteamVR_Input.actionsIn = new Valve.VR.ISteamVR_Action_In[] {
                    SteamVR_Actions.default_InteractUI,
                    SteamVR_Actions.default_Teleport,
                    SteamVR_Actions.default_GrabPinch,
                    SteamVR_Actions.default_GrabGrip,
                    SteamVR_Actions.default_Pose,
                    SteamVR_Actions.default_SkeletonLeftHand,
                    SteamVR_Actions.default_SkeletonRightHand,
                    SteamVR_Actions.default_Squeeze,
                    SteamVR_Actions.default_HeadsetOnHead,
                    SteamVR_Actions.default_speed,
                    SteamVR_Actions.default_move,
                    SteamVR_Actions.default_toggle_menu,
                    SteamVR_Actions.default_touchpadact,
                    SteamVR_Actions.default_LockRotation,
                    SteamVR_Actions.default_Tgrab,
                    SteamVR_Actions.default_GoScale};
            Valve.VR.SteamVR_Input.actionsOut = new Valve.VR.ISteamVR_Action_Out[] {
                    SteamVR_Actions.default_Haptic};
            Valve.VR.SteamVR_Input.actionsVibration = new Valve.VR.SteamVR_Action_Vibration[] {
                    SteamVR_Actions.default_Haptic};
            Valve.VR.SteamVR_Input.actionsPose = new Valve.VR.SteamVR_Action_Pose[] {
                    SteamVR_Actions.default_Pose};
            Valve.VR.SteamVR_Input.actionsBoolean = new Valve.VR.SteamVR_Action_Boolean[] {
                    SteamVR_Actions.default_InteractUI,
                    SteamVR_Actions.default_Teleport,
                    SteamVR_Actions.default_GrabPinch,
                    SteamVR_Actions.default_GrabGrip,
                    SteamVR_Actions.default_HeadsetOnHead,
                    SteamVR_Actions.default_move,
                    SteamVR_Actions.default_toggle_menu,
                    SteamVR_Actions.default_LockRotation,
                    SteamVR_Actions.default_Tgrab,
                    SteamVR_Actions.default_GoScale};
            Valve.VR.SteamVR_Input.actionsSingle = new Valve.VR.SteamVR_Action_Single[] {
                    SteamVR_Actions.default_Squeeze,
                    SteamVR_Actions.default_speed};
            Valve.VR.SteamVR_Input.actionsVector2 = new Valve.VR.SteamVR_Action_Vector2[] {
                    SteamVR_Actions.default_touchpadact};
            Valve.VR.SteamVR_Input.actionsVector3 = new Valve.VR.SteamVR_Action_Vector3[0];
            Valve.VR.SteamVR_Input.actionsSkeleton = new Valve.VR.SteamVR_Action_Skeleton[] {
                    SteamVR_Actions.default_SkeletonLeftHand,
                    SteamVR_Actions.default_SkeletonRightHand};
            Valve.VR.SteamVR_Input.actionsNonPoseNonSkeletonIn = new Valve.VR.ISteamVR_Action_In[] {
                    SteamVR_Actions.default_InteractUI,
                    SteamVR_Actions.default_Teleport,
                    SteamVR_Actions.default_GrabPinch,
                    SteamVR_Actions.default_GrabGrip,
                    SteamVR_Actions.default_Squeeze,
                    SteamVR_Actions.default_HeadsetOnHead,
                    SteamVR_Actions.default_speed,
                    SteamVR_Actions.default_move,
                    SteamVR_Actions.default_toggle_menu,
                    SteamVR_Actions.default_touchpadact,
                    SteamVR_Actions.default_LockRotation,
                    SteamVR_Actions.default_Tgrab,
                    SteamVR_Actions.default_GoScale};
        }
        
        private static void PreInitActions()
        {
            SteamVR_Actions.p_default_InteractUI = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/InteractUI")));
            SteamVR_Actions.p_default_Teleport = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Teleport")));
            SteamVR_Actions.p_default_GrabPinch = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/GrabPinch")));
            SteamVR_Actions.p_default_GrabGrip = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/GrabGrip")));
            SteamVR_Actions.p_default_Pose = ((SteamVR_Action_Pose)(SteamVR_Action.Create<SteamVR_Action_Pose>("/actions/default/in/Pose")));
            SteamVR_Actions.p_default_SkeletonLeftHand = ((SteamVR_Action_Skeleton)(SteamVR_Action.Create<SteamVR_Action_Skeleton>("/actions/default/in/SkeletonLeftHand")));
            SteamVR_Actions.p_default_SkeletonRightHand = ((SteamVR_Action_Skeleton)(SteamVR_Action.Create<SteamVR_Action_Skeleton>("/actions/default/in/SkeletonRightHand")));
            SteamVR_Actions.p_default_Squeeze = ((SteamVR_Action_Single)(SteamVR_Action.Create<SteamVR_Action_Single>("/actions/default/in/Squeeze")));
            SteamVR_Actions.p_default_HeadsetOnHead = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/HeadsetOnHead")));
            SteamVR_Actions.p_default_speed = ((SteamVR_Action_Single)(SteamVR_Action.Create<SteamVR_Action_Single>("/actions/default/in/speed")));
            SteamVR_Actions.p_default_move = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/move")));
            SteamVR_Actions.p_default_toggle_menu = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/toggle_menu")));
            SteamVR_Actions.p_default_touchpadact = ((SteamVR_Action_Vector2)(SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/default/in/touchpadact")));
            SteamVR_Actions.p_default_LockRotation = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/LockRotation")));
            SteamVR_Actions.p_default_Tgrab = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Tgrab")));
            SteamVR_Actions.p_default_GoScale = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/GoScale")));
            SteamVR_Actions.p_default_Haptic = ((SteamVR_Action_Vibration)(SteamVR_Action.Create<SteamVR_Action_Vibration>("/actions/default/out/Haptic")));
        }
    }
}

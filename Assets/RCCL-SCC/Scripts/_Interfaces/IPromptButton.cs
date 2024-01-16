using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPromptButton
{
    PromptMenuType promptMenuType { get; set; }
    PromptAction promptActionType { get; set; }
}

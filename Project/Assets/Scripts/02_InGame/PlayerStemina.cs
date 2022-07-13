using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerStemina : MonoBehaviourPun
{
    public bool isWarm = false;

    private float hp = 1f;

    #region WetValue
    private float _wetValue = .0f;

    public float wetValue
    {
        set
        {
            _wetValue = value;
            CalcStamina();
        }
        get => _wetValue;
    }
    private float wetTimer = .0f;
    private float wetDelay = 2f;
    #endregion

    #region ThirstValue
    private float _thirstValue = 1f;

    public float thirstValue
    {
        set
        {
            _thirstValue = value;
            CalcStamina();
        }
        get => _thirstValue;
    }

    private float thirstyFulldelay = 30f;
    private float thirstyFullTimer = .0f;
    private float thirstyDownTimer = .0f;
    private float thirstyDelay = 4.5f;
    #endregion

    #region HungryValue
    private float _hungryValue = 1f;

    public float hungryValue
    {
        set
        {
            _hungryValue = value;
            CalcStamina();
        }
        get => _hungryValue;
    }

    private float hungryFulldelay = 30f;
    private float hungryFullTimer = .0f;
    private float hungryDownTimer = .0f;
    private float hungryDelay = 2.1f;
    #endregion

    #region ColdValue
    private float _warmValue = 1f;

    public float warmValue
    {
        set
        {
            _warmValue = value;
            CalcStamina();
        }
        get => _warmValue;
    }

    private float warmFulldelay = 30f;
    private float warmFullTimer = .0f;
    private float warmDownTimer = .0f;
    private float warmDelay = 3.5f;
    #endregion

    bool normalPlayer = false;

    float removeTimer = 60f;
    float removeDelay = 60f;
    bool canRemove = false;
    private void Awake()
    {
        normalPlayer = !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isMurder"] && !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"];
        EventManager.AddEvent("Item :: Fish", (p) =>
        {
            if (normalPlayer)
            {
                FillHungry(.35f);
                EventManager.SendEvent("Inventory :: Remove", "0000");
            }
            else
            {
                EventManager.SendEvent("Inventory :: Remove", "0000");
            }
        });
        EventManager.AddEvent("Item :: GrilledFish", (p) =>
        {
            if (normalPlayer)
            {
                FillHungry(.75f);
                EventManager.SendEvent("Inventory :: Remove", "0001");
            }
            else
            {
                EventManager.SendEvent("Inventory :: Remove", "0001");
            }
        });
        EventManager.AddEvent("Item :: EmptyBottle", (p) =>
        {
            if (normalPlayer)
            {

            }
            else
            {
                EventManager.SendEvent("Inventory :: Remove", "0002");
            }
        });
        EventManager.AddEvent("Item :: FullBottle", (p) =>
        {
            if (normalPlayer)
            {
                if ((bool)EventManager.GetData("Inventory >> TryChange", "0003", "0002"))
                {
                    FillThirsty(.8f);
                    EventManager.SendEvent("Inventory :: Change", "0003", "0002");
                }
            }
            else
            {
                EventManager.SendEvent("Inventory :: Remove", "0003");
            }
        });
        EventManager.AddEvent("Item :: HandWarmer", (p) =>
        {
            if (normalPlayer)
            {
                EventManager.SendEvent("Inventory :: Change", "0004", "0005");
                EventManager.SendEvent("Inventory :: HandWarmer", EventManager.GetData("Inventory >> FindIndex", "0005"));
            }
            else
            {
                EventManager.SendEvent("Inventory :: Remove", "0004");
            }
        });
        EventManager.AddEvent("Item :: UsingHandWarmer", (p) =>
        {
            if (normalPlayer)
            {

            }
            else
            {
                EventManager.SendEvent("Inventory :: Remove", "0005");
            }
        });
        EventManager.AddEvent("Item :: ColdHandWarmer", (p) =>
        {
            EventManager.SendEvent("Inventory :: Remove", "0006");
        });
        EventManager.AddEvent("Player :: EnterWarmZone", (p) =>
        {
            isWarm = true;
        });
        EventManager.AddEvent("Player :: ExitWarmZone", (p) =>
        {
            isWarm = false;
        });
        EventManager.AddEvent("Player :: EnterWet", (p) =>
        {
            wetValue = 1;
        });
        EventManager.AddEvent("Player :: ExitWet", (p) =>
        {
            wetValue = 1;
        });
        EventManager.AddEvent("Player :: FallingDamage", (p) =>
        {
            float damage = (float)p[0] / 100f;
            GetHit(-damage, DieMessage.Falling);
        });
        EventManager.AddEvent("Player :: BreathDamage", (p) =>
        {
            GetHit((float)p[0], DieMessage.Breath);
        });
        EventManager.AddData("Player >> CanRemove", (p) => canRemove);
        EventManager.AddEvent("Player :: SetRemoveItemTimer", (p) =>
        {
            EventManager.SendEvent("Inventory :: SetRemoveItemTimer", removeDelay);
            removeTimer = .0f;
        });
    }

    private void Update()
    {
        if ((bool)EventManager.GetData("InGameUI >> VoteUIActive")) return;
        if ((bool)EventManager.GetData("InGameData >> FinishVoteAnimationPlaying")) return;
        if (normalPlayer && !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"])
        {
            ThirstValue();
            HungryValue();
            WarmValue();
            WetValue();
        }
        if (!normalPlayer && !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"])
        {
            RemoveItemDelay();
        }
    }

    private void RemoveItemDelay()
    {
        removeTimer += Time.deltaTime;
        canRemove = removeTimer >= removeDelay;
    }

    private void CalcStamina()
    {
        EventManager.SendEvent("Refresh Stamina", hp);
        EventManager.SendEvent("Refresh Thirsty", thirstValue);
        EventManager.SendEvent("Refresh Hungry", hungryValue);
        EventManager.SendEvent("Refresh Cold", warmValue);
        EventManager.SendEvent("Refresh Wet", wetValue);
    }

    private void FillThirsty(float value)
    {
        float remainValue = 1 - thirstValue;
        if (remainValue >= value)
        {
            thirstValue += value;
        }
        else
        {
            thirstValue = 1;
            if (thirstyFullTimer >= thirstyFulldelay) thirstyFullTimer = thirstyFulldelay;
            thirstyFullTimer -= thirstyFulldelay * (value - remainValue);
            if (thirstyFullTimer <= 0) thirstyFullTimer = 0;
        }
    }

    private void FillHungry(float value)
    {
        float remainValue = 1 - hungryValue;
        if (remainValue >= value)
        {
            hungryValue += value;
        }
        else
        {
            hungryValue = 1;
            if (hungryFullTimer >= hungryFulldelay) hungryFullTimer = hungryFulldelay;
            hungryFullTimer -= hungryFulldelay * (value - remainValue);
            if (hungryFullTimer <= 0) hungryFullTimer = 0;
        }
    }

    private void FillWarm(float value)
    {
        float remainValue = 1 - warmValue;
        if (remainValue >= value)
        {
            warmValue += value;
        }
        else
        {
            warmValue = 1;
            if (warmFullTimer >= warmFulldelay) warmFullTimer = warmFulldelay;
            warmFullTimer -= warmFulldelay * (value - remainValue);
            if (warmFullTimer <= 0) warmFullTimer = 0;
        }
    }

    private void ThirstValue()
    {
        if (thirstyFullTimer >= thirstyFulldelay)
        {
            if (thirstyDownTimer >= thirstyDelay)
            {
                thirstyDownTimer -= thirstyDelay;
                thirstValue -= .04f;
                if (thirstValue < 0)
                {
                    thirstValue = 0;
                    GetHit(.02f, DieMessage.Hungry);
                }
            }
            else
            {
                thirstyDownTimer += Time.deltaTime;
            }
        }
        else
        {
            thirstyFullTimer += Time.deltaTime;
            thirstyDownTimer = 0;
        }
        if (thirstValue >= 1f)
        {
            GetHit(-.02f * Time.deltaTime, DieMessage.None);
        }
    }

    private void HungryValue()
    {
        if (hungryFullTimer >= hungryFulldelay)
        {
            if (hungryDownTimer >= hungryDelay)
            {
                hungryDownTimer -= hungryDelay;
                hungryValue -= .025f;
                if (hungryValue < 0)
                {
                    hungryValue = 0;
                    GetHit(.02f, DieMessage.Hungry);
                }
            }
            else
            {
                hungryDownTimer += Time.deltaTime;
            }
        }
        else
        {
            hungryFullTimer += Time.deltaTime;
            hungryDownTimer = 0;
        }
        if (hungryValue >= 1f)
        {
            GetHit(-.02f * Time.deltaTime, DieMessage.None);
        }
    }

    private void WarmValue()
    {
        bool warmItem = (bool)EventManager.GetData("Inventory >> HasItem", "0005");
        if (isWarm)
        {
            warmDownTimer = .0f;
            FillWarm(.1f * Time.deltaTime);
        }
        if (warmItem)
        {
            warmDownTimer = .0f;
            FillWarm(.04f * Time.deltaTime);
        }
        if (isWarm || warmItem) return;
        if (warmFullTimer >= warmFulldelay)
        {
            if (warmDownTimer >= warmDelay)
            {
                warmDownTimer -= warmDelay;
                warmValue -= .03f * (1 + wetValue);
                if (warmValue < 0)
                {
                    warmValue = 0;
                    GetHit(.02f, DieMessage.Hungry);
                }
            }
            else
            {
                warmDownTimer += Time.deltaTime;
            }
        }
        else
        {
            warmFullTimer += Time.deltaTime;
            warmDownTimer = 0;
        }
    }

    private void WetValue()
    {
        if (wetValue > 0)
        {
            if (wetTimer >= wetDelay)
            {
                wetTimer -= wetDelay;
                wetValue -= isWarm ? .15f : .02f;
            }
            else
            {
                wetTimer += Time.deltaTime;
            }
        }
        else
        {
            wetValue = 0;
        }
    }

    private void GetHit(float value, DieMessage msg)
    {
        hp -= value;
        if (hp <= 0f && !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"])
        {
            EventManager.SendEvent("InGameUI :: Die", msg);
            EventManager.SendEvent("Data :: Die", msg, true, true);
            hp = 0f;
            thirstValue = .0f;
            hungryValue = .0f;
            warmValue = .0f;
            wetValue = .0f;
        }
        else if (hp > 1f)
        {
            hp = 1f;
        }
        CalcStamina();
    }
}

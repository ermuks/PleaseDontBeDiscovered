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
    private float hungryDelay = 2.8f;
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

    private void Awake()
    {
        normalPlayer = !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isMurder"] && !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"];
        EventManager.AddEvent("Item :: Fish", (p) =>
        {
            if (normalPlayer)
            {
                FillHungry(.2f);
            }
        });
        EventManager.AddEvent("Item :: GrilledFish", (p) =>
        {
            if (normalPlayer)
            {
                FillHungry(.4f);
            }
        });
        EventManager.AddEvent("Item :: EmptyBottle", (p) =>
        {
            if (normalPlayer)
            {
                EventManager.SendEvent("Player :: UseEmptyBottle");
            }
        });
        EventManager.AddEvent("Item :: FullBottle", (p) =>
        {
            if (normalPlayer)
            {
                FillThirsty(.5f);
                EventManager.SendEvent("Inventory :: Change", "0003", "0002", true);
            }
        });
        EventManager.AddEvent("Item :: HandWarmer", (p) =>
        {
            if (normalPlayer)
            {
                EventManager.SendEvent("Inventory :: Change", "0004", "0005", true);
            }
        });
        EventManager.AddEvent("Item :: UsingHandWarmer", (p) =>
        {
            if (normalPlayer)
            {
                
            }
        });
        EventManager.AddEvent("Item :: ColdHandWarmer", (p) =>
        {
            if (normalPlayer)
            {
                
            }
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
            float damage = (float)p[0] / 15f;
            GetHit(-damage, DieMessage.Falling);
        });
    }

    private void Update()
    {
        if ((bool)EventManager.GetData("InGameUI >> VoteUIActive")) return;
        if (normalPlayer && !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"])
        {
            ThirstValue();
            HungryValue();
            WarmValue();
            WetValue();
        }
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
                hungryValue -= .075f;
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
        if (isWarm)
        {
            warmDownTimer = .0f;
            FillWarm(.1f * Time.deltaTime);
            return;
        }
        if (warmFullTimer >= warmFulldelay)
        {
            if (warmDownTimer >= warmDelay)
            {
                warmDownTimer -= warmDelay;
                warmValue -= .05f * (1 + wetValue);
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
                wetValue -= isWarm ? .15f : .05f;
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
            EventManager.SendEvent("Player :: Die", msg);
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

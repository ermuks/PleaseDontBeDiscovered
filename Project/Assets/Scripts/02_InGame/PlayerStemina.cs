using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerStemina : MonoBehaviourPun
{
    public string playerName = "";

    private Inventory inventory;

    public bool isWarm = false;

    private float hp = 1f;

    #region WetValue
    public float wetValue = .0f;
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
    private float _coldValue = 1f;

    public float warmValue
    {
        set
        {
            _coldValue = value;
            CalcStamina();
        }
        get => _coldValue;
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
    }

    private void Update()
    {
        if (normalPlayer)
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
            if (thirstyDownTimer >= thirstyFulldelay) thirstyDownTimer = thirstyFulldelay;
            thirstyDownTimer -= thirstyFulldelay * (value - remainValue);
            if (thirstyDownTimer <= 0) thirstyDownTimer = 0;
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
            if (hungryDownTimer >= hungryFulldelay) hungryDownTimer = hungryFulldelay;
            hungryDownTimer -= hungryFulldelay * (value - remainValue);
            if (hungryDownTimer <= 0) hungryDownTimer = 0;
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
            if (warmDownTimer >= warmFulldelay) warmDownTimer = warmFulldelay;
            warmDownTimer -= warmFulldelay * (value - remainValue);
            if (warmDownTimer <= 0) warmDownTimer = 0;
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
                    GetHit(.02f);
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
        if (thirstValue > .8f)
        {
            GetHit(-.02f);
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
                    GetHit(.02f);
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
        if (hungryValue > .8f)
        {
            GetHit(-.02f);
        }
    }

    private void WarmValue()
    {
        if (isWarm) return;
        if (warmFullTimer >= warmFulldelay)
        {
            if (warmDownTimer >= warmDelay)
            {
                warmDownTimer -= warmDelay;
                warmValue -= .05f * (1 + wetValue);
                if (warmValue < 0)
                {
                    warmValue = 0;
                    GetHit(.02f);
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
                wetValue -= .05f;
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

    private void GetHit(float value)
    {
        hp -= value;
        if (hp <= 0)
        {
            EventManager.SendEvent("InGameUI :: Die", DieMessage.Hungry);
            EventManager.SendEvent("Player :: Die", DieMessage.Hungry);
            hp = 0;
        }
        else if (hp >= 1)
        {
            hp = 1;
        }
    }
}

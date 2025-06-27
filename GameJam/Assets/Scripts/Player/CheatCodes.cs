using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Linq;
using TMPro;
using Random = UnityEngine.Random;

[Serializable]
public class Cheat
{
    [SerializeField] private CheatCodes.Cheats[] names;
    [SerializeField] private float cooldown;
    [SerializeField] private float activeTime;
    public float activeTimer;
    public float cooldownTimer = 0;
    private bool _isActive = false, _onCooldown = false;

    public Cheat(CheatCodes.Cheats[] names, int cd)
    {
        this.names = names;
        cooldown = cd;
    }
    public CheatCodes.Cheats[] GetNames()
    {
        return names;
    }
    public float GetCooldown()
    {
        return cooldown;
    }
    public bool IsActive()
    {
        return _isActive;
    }
    public bool OnCooldown()
    {
        return _onCooldown;
    }
    public void ToggleOnCooldown()
    {
        _onCooldown = !_onCooldown;
    }
    public void ToggleActive()
    {
        _isActive = !_isActive;
    }
    public float GetActiveTime()
    {
        return activeTime;
    }
}

public class CheatCodes : MonoBehaviour
{
    [SerializeField] private int historyLength = 25;
    [SerializeField] private LevelLoader _levelLoader;
    private MouseInput _keyboardInput;
    private List<GameObject> _cheatTexts;
    private List<GameObject> _toDelete;
    private List<string> _keyHistory;

    private readonly float _textXRange = Screen.width/4;
    private readonly float _textYRange = Screen.height/4;

    [SerializeField] private List<Cheat> cheatsList;
    private GameObject _canvas;

    public enum Cheats
    {
        givememorehealth,
        morehealth,
        healthpls,
        noclip,
        tcl,
        killall,
        destroyall,
        godmode,
        tgm,
        togglegodmode,
        nextlevel,
        complete,
        completelevel,
        beatlevel,
        beat,
        givemescore,
        givemepoints,
        givememorepoints,
        pointspls,
        morepoints,
        strengthpotion,
        attackpotion,
        strengthserum,
        attackserum,
        speed
    };

    private void Awake()
    {
        _keyboardInput = new MouseInput();
        _keyHistory = new List<string>();
        _cheatTexts = new List<GameObject>();
        _toDelete = new List<GameObject>();
    }
    private void Start()
    {
        _canvas = GameObject.Find("Player UI");
        _keyboardInput.Cheats.Keys.performed += (ctx) => KeyPress(ctx.control.name);
    }
    private void OnEnable()
    {
        _keyboardInput.Enable();
    }
    private void OnDisable()
    {
        _keyboardInput.Disable();
    }

    private void Update()
    {
        foreach (var cheat in cheatsList)
        {
            if (cheat.cooldownTimer > 0)
                cheat.cooldownTimer -= Time.deltaTime;
            
            if (cheat.cooldownTimer <= 0 && cheat.OnCooldown())
            {
                PopupText(cheat.GetNames()[0] + " off cooldown!");
                cheat.ToggleOnCooldown();
                cheat.cooldownTimer = 0;
            }
               

            // active time of 0 indicates it is not an activatable cheat. IE - give health just gives health,
            // no need for a timer for that
            if (cheat.IsActive() && cheat.GetActiveTime() != 0)
            {
                if (cheat.activeTimer > 0)
                    cheat.activeTimer -= Time.deltaTime;
                
                if (cheat.activeTimer <= 0)
                {
                    PopupText(cheat.GetNames()[0] + " deactivated!");
                    cheat.activeTimer = 0;
                    cheat.ToggleActive();
                    ActivateCheat(cheat.GetNames()[0]);
                }
            }
        }

        foreach (var textObject in _cheatTexts)
        {
            textObject.transform.position += new Vector3(0, -0.1f, 0);
            var text = textObject.GetComponent<TMPro.TextMeshProUGUI>();
            text.faceColor = new Color32(text.faceColor.r, text.faceColor.g, text.faceColor.b, (byte)(text.faceColor.a - 1));
            if (text.faceColor.a <= 0)
            {
                _toDelete.Add(textObject);
            }
        }
        foreach (var deleteObject in _toDelete)
        {
            _cheatTexts.Remove(deleteObject);
            Destroy(deleteObject);
        }
        if (_toDelete.Count > 0)
        {
            _toDelete.Clear();
        }
    }

    private void PopupText(string text)
    {
        var xPos = Random.Range(-_textXRange, _textXRange);
        var yPos = Random.Range(-_textYRange, _textYRange);
        var canvasTransform = _canvas.transform;
        _cheatTexts.Add(GameObject.Instantiate((GameObject)Resources.Load("CheatText"), canvasTransform));
        _cheatTexts.Last<GameObject>().transform.SetParent(canvasTransform);
        _cheatTexts.Last<GameObject>().transform.position += new Vector3(xPos, yPos, 0);
        var cheatText = _cheatTexts.Last<GameObject>().GetComponent<TMPro.TextMeshProUGUI>();
        cheatText.autoSizeTextContainer = true;
        cheatText.horizontalAlignment = HorizontalAlignmentOptions.Center;
        cheatText.text = text;
    }
    
    private void KeyPress(string key)
    {
        if (_keyHistory.Count > historyLength)
        {
            _keyHistory.RemoveAt(0);
        }
        _keyHistory.Add(key);

        var curHistory = string.Join("", _keyHistory);
        foreach (var cheat in cheatsList)
        {
            foreach (var cheatName in cheat.GetNames())
            {
                if (curHistory.Contains(cheatName.ToString()))
                {
                    if (cheat.cooldownTimer <= 0)
                    {
                        PopupText("Activated " + cheat.GetNames()[0] + "!");
                        
                        ActivateCheat(cheatName);
                        cheat.cooldownTimer = cheat.GetCooldown();
                        cheat.ToggleOnCooldown();
                        cheat.activeTimer = cheat.GetActiveTime();
                        cheat.ToggleActive();
                    }
                    else
                        PopupText(cheat.GetNames()[0] + " still on cooldown for " + Mathf.RoundToInt(cheat.cooldownTimer) + " seconds.");
                    
                    _keyHistory.Clear();
                    break;
                }
            }
        }
    }

    private void ActivateCheat(Cheats cheat)
    {
        switch (cheat)
        {
            case Cheats.givememorehealth:
            case Cheats.morehealth:
            case Cheats.healthpls:
                var ply = gameObject.GetComponent<PlayerCombat>();
                ply.AddHealth(ply.maxHealth);
                break;
            case Cheats.noclip:
            case Cheats.tcl:
                var playerCollider = GameObject.FindWithTag("Player").GetComponent<BoxCollider2D>();
                playerCollider.isTrigger = !playerCollider.isTrigger;
                break;
            case Cheats.destroyall:
            case Cheats.killall:
                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<EnemyAttack>().HitEnemy(999, Vector3.zero, null);
                }
                break;
            case Cheats.godmode:
            case Cheats.tgm:
            case Cheats.togglegodmode:
                var ply1 = gameObject.GetComponent<PlayerCombat>();
                ply1.ToggleGodMode();
                break;
            case Cheats.nextlevel:
            case Cheats.complete:
            case Cheats.beatlevel:
            case Cheats.completelevel:
            case Cheats.beat:
                _levelLoader.LoadNextLevel();
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case Cheats.givemescore:
            case Cheats.givemepoints:
                GameObject.FindWithTag("Player").GetComponent<ScoreManager>().AddPoints(750);
                break;
            case Cheats.strengthpotion:
            case Cheats.attackpotion:
            case Cheats.strengthserum:
            case Cheats.attackserum:
                GameObject.FindWithTag("Player").GetComponent<PlayerCombat>().ToggleAttackBoost();
                break;
            case Cheats.speed:
                var playerController = GameObject.FindWithTag("Player").GetComponent<IsoController>();
                if (playerController.GetMoveSpeed() == playerController.GetBaseSpeed())
                    playerController.SetSpeed(playerController.GetBaseSpeed() * 3);
                else
                    playerController.SetSpeed(playerController.GetBaseSpeed());
                break;
            default:
                break;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warmodes : MonoBehaviour
{

    [SerializeField]
    private fairyController _fairyController;
    [SerializeField, Range(0, 1)]
    private float _bulletTimeScale;
    [SerializeField, Range(0, 1)]
    private float _normalTimeScale;
    private bool _isActive = false;
    private CharacterDebuffs _characterDebuffs;
    private CollisionHandler _collisionHandler;
    private characterController _cc;
    private float _curHP;
    private GameObject _activeTurret;

    [SerializeField, Tooltip("Defines the amount of the color/hp loss per second. Max Coloramount/Fairry = 1")]
    private float _colorLossAmount = 0.05f;
    [SerializeField, Tooltip("The Ice Shooting turret for the Bluewarmode")]
    private GameObject _turretPref;

    private characterController.playerStates _curWarMode;

    public bool IsActive { get { return _isActive; } }
    public characterController.playerStates CurWarMode { get { return _curWarMode;  } }

    private void Start()
    {
        _characterDebuffs = this.GetComponent<CharacterDebuffs>();
        _collisionHandler = this.GetComponent<CollisionHandler>();
        _cc = this.GetComponent<characterController>();
    }

    private void Update()
    {
        //early out if not in use
        if (!_isActive)
            return;

        _curHP =  PlayerDamageHandler.GetDamage(_colorLossAmount*Time.deltaTime, PlayerUttillitys.GetPlayerColor(_curWarMode), _fairyController);
        if (_curHP <= 0)
        {
            DeactivateWarmode(); 
        }
    }

    public void UseWarmode(characterController.playerStates color)
    {
        switch (color)
        {
            case characterController.playerStates.blue:
                BlueWarMode(); 
                break;
            case characterController.playerStates.red:
                RedWarMode(); 
                break;
            case characterController.playerStates.yellow:
                YellowWarMode();
                break; 
            default:
                Debug.LogWarning("Other stuff needs to be implemented/Or allready in warmode");
                break; 
        }
    }

    private void RedWarMode()
    {
        _isActive = true;
        _cc.StatusData.currentState = characterController.playerStates.burntRed;
        _curWarMode = _cc.StatusData.currentState;
        _characterDebuffs.setOnFire();
    }

    private void BlueWarMode()
    {
        if (_activeTurret == null)
        {
            _isActive = true;
            _cc.StatusData.currentState = characterController.playerStates.burntBlue;
            _curWarMode = _cc.StatusData.currentState; 
            _activeTurret = Instantiate(_turretPref, transform.position, Quaternion.identity);
        }
    }

    private void YellowWarMode()
    {
        Debug.Log("yellow war mode"); 
        _cc.StatusData.currentState = characterController.playerStates.burntYellow;
        var enemys = FindObjectsOfType<SimpleAI>(); 
        foreach (SimpleAI ai in enemys)
        {
            ai.SetTimeScale(_bulletTimeScale); 
        }
   
    }

    private void GreenWarMode()
    {
        throw new System.NotImplementedException(); 
    }

    private void DeactivateWarmode()
    {
        _isActive = false; 
        _fairyController.debugSetPlayerState(); 
        switch (_curWarMode)
        {
            case characterController.playerStates.burntBlue:
                Destroy(_activeTurret);
                _activeTurret = null;
                break;
            case characterController.playerStates.burntYellow:
                var enemys = FindObjectsOfType<SimpleAI>();
                foreach (SimpleAI ai in enemys)
                {
                    ai.SetTimeScale(_normalTimeScale);
                }

                break;
            default:
                Debug.LogWarning("Other stuff is not implemented, does not need specific stuff to be done to deactivate");
                break;
        }
    }

}

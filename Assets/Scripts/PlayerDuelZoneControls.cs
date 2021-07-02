using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerDuelZoneControls : MonoBehaviour
{
    private GameObject healthBar;
    private PhotonView photonView;
    private Vector2 touchStartPosition, touchEndPosition;
    private Touch theTouch;
    private bool isShootingReady = true;

    public GameObject shotPrefab;
    public GameObject heartPrefab;

    private int playerHealth = 100;

    public void LessPlayerHealth(int shootingPower)
    {
        playerHealth -= shootingPower;
    }

    public int GetPlayerHealth()
    {
        return playerHealth;
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        healthBar = GameObject.Find("HealthBar");
        healthBar.GetComponent<SpriteRenderer>().color = Color.blue;
    }

    void Update()
    {
        ControlPlayerForSmartphone();
        ControlPlayerForPC();
    }

    public void ChangePlayerHealthBar()
    {
        healthBar.transform.localScale = new Vector3(healthBar.transform.localScale.x - 15f, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    private IEnumerator ToggleIsShootingReady()
    {
        isShootingReady = false;
        yield return new WaitForSeconds(0.5f);
        isShootingReady = true;
    }

    private bool IsCurrentPlayer()
    {
        return photonView.IsMine;
    }

    [PunRPC]
    private void CreateShotCoroutine(float rotation, Vector3 position)
    {
        StartCoroutine(CreateShot(rotation, position));
    }

    [PunRPC]
    private IEnumerator CreateShot(float rotation, Vector3 position)
    {
        GameObject shot = Instantiate(shotPrefab, position + new Vector3(rotation * 0.5f, 0, 0), Quaternion.identity);
        Rigidbody2D rigidbody2DShot = shot.GetComponent<Rigidbody2D>();
        rigidbody2DShot.AddForce(new Vector2(rotation * 8f, 0), ForceMode2D.Impulse);
        yield return new WaitForSeconds(3f);
        Destroy(shot);
    }

    private void ShootPlayer()
    {
        if (!IsCurrentPlayer()) return;
        StartCoroutine(ToggleIsShootingReady());
        float rotation = transform.rotation.y == 0 ? 1 : -1;
        Vector3 position = transform.position;
        Debug.Log("Rotation: ");
        Debug.Log(rotation);
        Debug.Log("Position: ");
        Debug.Log(position);
        photonView.RPC("CreateShotCoroutine", RpcTarget.All, rotation, position);
    }

    private void ControlPlayerForSmartphone()
    {
        if (Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);

            if (theTouch.phase == TouchPhase.Began)
            {
                touchStartPosition = theTouch.position;
            }
            else
            {
                if (theTouch.phase == TouchPhase.Moved || theTouch.phase == TouchPhase.Ended)
                {
                    touchEndPosition = theTouch.position;

                    float x = touchEndPosition.x - touchStartPosition.x;
                    float y = touchEndPosition.y - touchStartPosition.y;

                    if (Mathf.Abs(x) == 0 && Mathf.Abs(y) == 0 && isShootingReady)
                    {
                        ShootPlayer();
                    }
                    else
                    {
                        if (Mathf.Abs(x) > Mathf.Abs(y))
                        {
                            if (x > 0) //right
                            {
                                MovePlayerRight();
                            }
                            else //left
                            {
                                MovePlayerLeft();
                            }
                        }
                        else
                        {
                            if (y > 0) //jump
                            {
                                JumpPlayer();
                            }
                        }
                    }
                }
            }
        }
    }

    private void ControlPlayerForPC()
    {
        if (Input.GetKey(KeyCode.D)) //right
        {
            MovePlayerRight();
        }

        if (Input.GetKey(KeyCode.A)) //left
        {
            MovePlayerLeft();
        }

        if (Input.GetKeyDown(KeyCode.W)) //jump
        {
            JumpPlayer();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isShootingReady) //shoot
        {
            ShootPlayer();
        }
    }

    private void MovePlayerLeft()
    {
        if (!IsCurrentPlayer()) return;
        transform.rotation = new Quaternion(0, 180, 0, 0);
        transform.Translate(Time.deltaTime * 3, 0, 0);
    }

    private void MovePlayerRight()
    {
        if (!IsCurrentPlayer()) return;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.Translate(Time.deltaTime * 3, 0, 0);
    }

    private void JumpPlayer()
    {
        if (!IsCurrentPlayer() || transform.position.y >= -0.75) return;
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(new Vector2(0, 5f), ForceMode2D.Impulse);
    }
}

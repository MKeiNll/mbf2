using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Transform Camera;

    private float Speed = 55;
    private float JumpPower = 5f;
    private float MaxAngularVelocity = 100;

    private float reducedSpeed;
    private float normalSpeed;
    private float jumpCooldown;
    private float jumpTime;

    private int level;

    private Rigidbody rigidBody;
    private bool isGrounded;
    private bool onIce;

    private int gemCount;
    private int gemsCollected;
    private Text gemText;

    private Transform checkpoint;

    private List<Collision> currentCollisions = new List<Collision>();

    private void Start()
    {
        Cursor.visible = false;
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.maxAngularVelocity = MaxAngularVelocity;

        Transform startPlatform = GameObject.FindGameObjectsWithTag("Start")[0].transform;
        transform.position = new Vector3(startPlatform.position.x, startPlatform.position.y + 2, startPlatform.position.z);

        // initLevel();

        reducedSpeed = Speed * 0.725f;
        normalSpeed = Speed;
        jumpCooldown = 0.02f;
        jumpTime = 0f;

        onIce = false;

        gemCount = GameObject.Find("Gems").transform.childCount;
        gemText = GameObject.Find("GemText").GetComponent<Text>();
        gemsCollected = 0;
        updateText();

        checkpoint = null;
    }

    private void Update()
    {
        print(gemsCollected);
        handleSpeed();
    }
	
	private void FixedUpdate()
    {
        if(rigidBody.position.y <= -20)
        {
            Loss();
        }
        rigidBody.inertiaTensorRotation = new Quaternion(0.01f, 0.01f, 0.01f, 1); //Endless roatation fix
        handleInputs();
    }

    private void handleInputs()
    {
        // if(onIce)
        // {
        //     rigidBody.AddTorque(Quaternion.Euler(0, Camera.rotation.eulerAngles.y, 0) * Vector3.right * Speed * 0.5f * Input.GetAxisRaw("Vertical"));
        //     rigidBody.AddTorque(Quaternion.Euler(0, Camera.rotation.eulerAngles.y, 0) * Vector3.back * Speed * 0.5f * Input.GetAxisRaw("Horizontal"));
        // }
        // else
        // {
            rigidBody.AddTorque(Quaternion.Euler(0, Camera.rotation.eulerAngles.y, 0) * Vector3.right * Speed * Input.GetAxisRaw("Vertical"));
            rigidBody.AddTorque(Quaternion.Euler(0, Camera.rotation.eulerAngles.y, 0) * Vector3.back * Speed * Input.GetAxisRaw("Horizontal"));
        // }
        if (Input.GetAxisRaw("Jump") != 0 && isGrounded && Time.time - jumpTime >= jumpCooldown)
        {
            jumpTime = Time.time;
            rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0, rigidBody.linearVelocity.z);
            rigidBody.linearVelocity += Vector3.up * JumpPower * Input.GetAxis("Jump");
        }
        if (Input.GetAxisRaw("Restart") != 0 )
        {
            Restart();
        }
    }

    private void handleSpeed()
    {
        if (Input.GetAxisRaw("Vertical") != 0 && Input.GetAxisRaw("Horizontal") != 0)
        {
            Speed = reducedSpeed;
        }
        else
        {
            Speed = normalSpeed;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        currentCollisions.Add(other);
        isGrounded = true;

        if (other.collider.tag.Contains("Ice"))
        {
            Collider collider = GetComponent<Collider>();
            collider.material.dynamicFriction = 0.15F;
            collider.material.staticFriction = 0.15F;

            onIce = true;
        }
        if (other.collider.tag.Contains("Checkpoint"))
        {
            checkpoint = other.transform;
        }
        if (other.collider.tag.Contains("Finish") && gemsCollected == gemCount)
        {
            Win();
        }

        if (other.collider.tag.Contains("Moving"))
        {
            transform.parent = other.transform;
        }
        else
        {
            transform.parent = null;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        currentCollisions.Remove(other);

        if(currentCollisions.Count == 0)
        {
            isGrounded = false;
        }
        if (other.collider.tag.Contains("Ice"))
        {
            Collider collider = GetComponent<Collider>();
            collider.material.dynamicFriction = 0.6F;
            collider.material.staticFriction = 0.6F;

            onIce = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gem")
        {
            other.gameObject.SetActive(false);
            gemsCollected++;
            updateText();
        }
    }

    private void Win()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        LoadNextLevel();
    }

    private void Loss()
    {
        if(checkpoint != null)
        {
            rigidBody.linearVelocity = new Vector3(0, 0, 0);
            transform.position = new Vector3(checkpoint.position.x, checkpoint.position.y + 1, checkpoint.position.z);
        }
        else
        {
            Restart();
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void initLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string levelNumber = sceneName.Substring(1);
        int.TryParse(levelNumber, out level);
    }

    public void LoadNextLevel()
    {
        string nextLevelName = "r" + (level + 1);
        SceneManager.LoadScene(nextLevelName);
    }

    private void updateText()
    {
        gemText.text = "GEMS FOUND: " + gemsCollected + "/" + gemCount;
    }
}

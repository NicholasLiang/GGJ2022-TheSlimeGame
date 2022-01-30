using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Platformer.Mechanics;

// Use plugin namespace
using HybridWebSocket;


public enum GameState
{
    connecting,
    onGoing,
    losing,
    winning
}

public enum ServerState
{
    ready,
    connecting,
    connected,
    error
}


public class Networking : MonoBehaviour
{
    public string server_url;
    public WebSocket ws;
    public int clientId;
    public int currentRoomId;

    public List<string> rooms;

    public Transform spawnPointRed;
    public Transform spawnPointBlue;

    public Rigidbody2D mainPrefab;
    public Rigidbody2D guestPrefab;

    public Color player1Color;
    public Color player2Color;

    public Rigidbody2D mainPlayer;
    public Rigidbody2D guestPlayer;

    public Vector2 guestPlayerSyncPos;
    public float guestPlayerVelocityX;
    public float guestPlayerJumpState;

    public GameObject light;

    public NetworkingFollowMainPlayer cam;

    public bool letStart;
    public bool isRed;

    GameState gameState;
    public ServerState serverState;

    private float width;
    private float height;

    public GameObject bulletPrefab;
    public PlayerController pc;

    public float bulletCD;
    private float previousShootingTime = 0;

    public float bulletSpeed;
    public float bulletHeightOffset;
    public Rigidbody2D redPlayer;
    public Rigidbody2D bluePlayer;

    public bool shootABullet;
    public int bulletDirection;
    public Vector3 bulletPosition;

    public bool isDead;

    public float deadWaitTime;

    void Awake()
    {
        clientId = -1;
        currentRoomId = -1;

        letStart = false;

        guestPlayerSyncPos = new Vector2(0,0);

        serverState = ServerState.ready;
        gameState = GameState.connecting;

        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;

        shootABullet = false;
        isDead = false;
    }

    void OnGUI()
    {
        // Compute a fontSize based on the size of the screen width.
        GUI.skin.label.fontSize = (int)(Screen.width / 50.0f);
        GUI.color = Color.red;

        GUI.Label(new Rect(20, 20, width, height * 0.25f),
            "Server State: " + serverState + ", clientId: " + clientId + ", currentRoomId: " +　currentRoomId);
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.connecting:
                if (letStart)
                {
                    letStart = false;
                    gameState = GameState.onGoing;
                    initGame(isRed);
                    ws.Send(Encoding.UTF8.GetBytes("Init Room Data/" + currentRoomId));
                }
                break;
            case GameState.onGoing:

                // bullet
                if (pc.isShooting && pc.isRed)
                {
                    if (Time.time - previousShootingTime > bulletCD)
                    {
                        previousShootingTime = Time.time;
                        ws.Send(Encoding.UTF8.GetBytes("Make a bullet/" + 
                                    currentRoomId + "/" +
                                    (pc.lookingRight ? 1 : -1) + "/" + 
                                    redPlayer.transform.position.x + "/" +
                                    redPlayer.transform.position.y + "/" +
                                    redPlayer.transform.position.z + "/"));
                    }
                }

                if (shootABullet)
                {
                    shootABullet = false;
                    GameObject newbullet = Instantiate(bulletPrefab);
                    newbullet.transform.position = bulletPosition + new Vector3(0, bulletHeightOffset, 0);
                    newbullet.GetComponent<Rigidbody2D>().velocity = new Vector2(bulletDirection * bulletSpeed, 0);
                }

                if (isDead)
                {
                    isDead = false;
                    pc.controlEnabled = false;
                    StartCoroutine(DeadEvent());
                }

                ws.Send(Encoding.UTF8.GetBytes("Request Room Data/" + currentRoomId));
                ws.Send(Encoding.UTF8.GetBytes("Send Local Data/" +
                                currentRoomId + "/" +
                                clientId + "/" +
                                (isRed ? 1 : 0) + "/" +
                                mainPlayer.position.x + "/" + 
                                mainPlayer.position.y + "/" +
                                mainPlayer.GetComponent<Animator>().GetFloat("velocityX") + "/" +
                                (mainPlayer.GetComponent<Animator>().GetBool("grounded") ? 1 : 0) + "/" +
                                "finished"));

                guestPlayer.MovePosition(guestPlayerSyncPos);
                guestPlayer.GetComponent<Animator>().SetFloat("velocityX", guestPlayerVelocityX);
                guestPlayer.GetComponent<Animator>().SetBool("grounded", guestPlayerJumpState == 1);

                break;
            case GameState.losing:

                break;
            case GameState.winning:

                break;
            default:
                break;
        }


    }

    public IEnumerator DeadEvent()
    {
        mainPlayer.GetComponent<Animator>().SetTrigger("death");
        guestPlayer.GetComponent<Animator>().SetTrigger("death");
        yield return new WaitForSeconds(deadWaitTime);
        redPlayer.position = spawnPointRed.position;
        bluePlayer.position = spawnPointBlue.position;
        pc.controlEnabled = true;

    }

    public IEnumerator GetServerState()
    {
        yield return new WaitForSeconds(3);
        if(ws.GetState() != WebSocketState.Open)
        {
            serverState = ServerState.error;
        }
        yield return null;
    }

    public void StartConnection(string url)
    {
        serverState = ServerState.connecting;
        ws = WebSocketFactory.CreateInstance(url);

        ws.OnOpen += () =>
        {
            serverState = ServerState.connected;
            Debug.Log("WS connected!");
            Debug.Log("WS state: " + ws.GetState().ToString());
            Debug.Log(ws.GetState() == WebSocketState.Open);

            // send a join request, start a new room or enter to room
            ws.Send(Encoding.UTF8.GetBytes("Hello from Unity 3D!"));

            RequestClientID();
            GetListOfRooms();
        };

        ws.OnMessage += (byte[] msg) =>
        {
            DecodeMessage(msg);
        };

        ws.Connect();

        StartCoroutine(GetServerState());
    }

    void DecodeMessage(byte[] msg)
    {

        string phrase = Encoding.Default.GetString(msg);
        string[] words = phrase.Split('/');

        if (words[0].Equals("You are client", System.StringComparison.Ordinal))
        {
            clientId = int.Parse(words[1]);
        }
        else if (words[0].Equals("Room Request", System.StringComparison.Ordinal))
        {
            rooms.Clear();
            for(var i = 1; i < words.Length; i++)
            {
                rooms.Add(words[i]);
            }
        } else if (words[0].Equals("You are in room", System.StringComparison.Ordinal))
        {
            currentRoomId = int.Parse(words[1]);
        }
        else if (words[0].Equals("You are Red", System.StringComparison.Ordinal))
        {
            isRed = true;
            letStart = true;

        }
        else if (words[0].Equals("You are Blue", System.StringComparison.Ordinal))
        {
            isRed = false;
            letStart = true;
        } else if (words[0].Equals("Game Data", System.StringComparison.Ordinal))
        {

            if(isRed)
            {
                // fill in blue
                guestPlayerSyncPos = new Vector2(float.Parse(words[5]), float.Parse(words[6]));

                guestPlayerVelocityX = float.Parse(words[7]);
                guestPlayerJumpState = int.Parse(words[8]);


            } else {
                // fill in red
                guestPlayerSyncPos = new Vector2(float.Parse(words[1]), float.Parse(words[2]));

                guestPlayerVelocityX = float.Parse(words[3]);
                guestPlayerJumpState = int.Parse(words[4]);
            }
        } else if (words[0].Equals("Shoot a bullet", System.StringComparison.Ordinal))
        {
            int client1 = int.Parse(words[1]);
            int client2 = int.Parse(words[2]);
            int direction = int.Parse(words[3]);

            float x = float.Parse(words[4]);
            float y = float.Parse(words[5]);
            float z = float.Parse(words[6]);

            if (clientId == client1 || clientId == client2)
            {
                Debug.Log("shoot a bullet");
                shootABullet = true;
                bulletDirection = direction;
                bulletPosition = new Vector3(x,y,z);
            }
        } else if (words[0].Equals("Dead Event", System.StringComparison.Ordinal))
        {
            Debug.Log("dead event");
            int client1 = int.Parse(words[1]);
            int client2 = int.Parse(words[2]);

            if (clientId == client1 || clientId == client2)
            {
                isDead = true;
            }
        }
    }

    public void initGame(bool isRed)
    {
        if (isRed)
        {
            mainPlayer = Instantiate(mainPrefab, spawnPointRed.position, Quaternion.identity) as Rigidbody2D;
            guestPlayer = Instantiate(guestPrefab, spawnPointBlue.position, Quaternion.identity) as Rigidbody2D;

            redPlayer = mainPlayer;
            bluePlayer = guestPlayer;
            mainPlayer.GetComponent<SpriteRenderer>().color = player1Color;
            Instantiate(light, mainPlayer.transform);
            pc = mainPlayer.GetComponent<PlayerController>();
            mainPlayer.GetComponent<PlayerController>().isRed = true;;
            guestPlayer.GetComponent<SpriteRenderer>().color = player2Color;
        } else {
            mainPlayer = Instantiate(mainPrefab, spawnPointBlue.position, Quaternion.identity) as Rigidbody2D;
            guestPlayer = Instantiate(guestPrefab, spawnPointRed.position, Quaternion.identity) as Rigidbody2D;

            bluePlayer = mainPlayer;
            redPlayer = guestPlayer;

            mainPlayer.GetComponent<SpriteRenderer>().color = player2Color;
            guestPlayer.GetComponent<SpriteRenderer>().color = player1Color;
            redPlayer = guestPlayer;
            Instantiate(light, guestPlayer.transform);
        }

        cam.Follow(mainPlayer);
    }

    public void StopConnection()
    {
        ws.Close();
    }

    public void RequestClientID()
    {
        ws.Send(Encoding.UTF8.GetBytes("Request Client ID"));
    }

    public void GetListOfRooms()
    {
        ws.Send(Encoding.UTF8.GetBytes("Request Room List"));
    }

    public void CreateRoom(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        ws.Send(Encoding.UTF8.GetBytes("Create Room/" + name + "/" + clientId));

        GetListOfRooms();
    }

    public void JoinRoom(string id)
    {
        if (string.IsNullOrEmpty(id))
            return;

        ws.Send(Encoding.UTF8.GetBytes("Join Room/" + id + "/" + clientId));
    }

    public void StartRoom()
    {
        ws.Send(Encoding.UTF8.GetBytes("Start Room/" + currentRoomId + "/" + clientId));
    }

    public void Dead()
    {
        ws.Send(Encoding.UTF8.GetBytes("You have dead/" + currentRoomId));
    }

    void OnApplicationQuit()
    {
        if (!(ws.GetState() == WebSocketState.Open))
            return;

        ws.Close();
    }
}

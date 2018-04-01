using UnityEngine;

public class InputManager : MonoBehaviour {

    static InputManager _instance;

    public static InputManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<InputManager>();

                if(_instance == null)
                    _instance = new GameObject("InputManager").AddComponent<InputManager>();
            }
            return _instance;
        }
    }

    private float _HorizontalInput;
    private float _VerticalInput;         

    public float HorizontalInput
    {
        get
        {
            return _HorizontalInput;
        }
    }

    public float VerticalInput
    {
        get
        {
            return _VerticalInput;
        }
    }

    private void Update()
    {
        bool goRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        bool goLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool goUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        bool goDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);

        if (goRight)  // 玩家想要向右移动
        {
            _HorizontalInput = 1;
        } 

        if(goLeft)
        {
            _HorizontalInput = -1;
        }

        if(!goRight && !goLeft)
        {
            _HorizontalInput = 0;
        }

        if(goUp)
        {
            _VerticalInput = 1;
        }

        if(goDown)
        {
            _VerticalInput = -1;
        }

        if(!goUp && !goDown)
        {
            _VerticalInput = 0;
        }
    }

}

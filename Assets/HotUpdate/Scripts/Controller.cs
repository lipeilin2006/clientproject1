using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Controller : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    bool isWalking = false;
    bool isRunning = false;
    bool isJumping = false;

    //因为unity编译时会删减未使用的代码，本项目热更场景并未加入到需要编译的场景中，因此要用这种方法确保Cinemachine在热更时能正常加载
    private void HoldAssembly()
    {
        CinemachineBrain brain = new();
    }
	private void Awake()
	{
		HoldAssembly();
	}
	void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	private void FixedUpdate()
	{
        this.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
        if (!isJumping)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isJumping = true;
                StartCoroutine(Jump());
            }
            else
            {
                if (Input.GetKey(KeyCode.W))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (!isRunning)
                        {
                            isRunning = true;
                            isWalking = false;
                            UpdateAnimatorVars();
                            animator.CrossFade("run", 0.1f);
                        }
                    }
                    else
                    {
                        if (!isWalking)
                        {
                            isWalking = true;
                            isRunning = false;
                            UpdateAnimatorVars();
                            animator.CrossFade("walk", 0.1f);
                        }
					}
                }
                else
                {
                    if (isRunning || isWalking)
                    {
                        isWalking = false;
                        isRunning = false;
                        UpdateAnimatorVars();
                        animator.CrossFade("idle", 0.1f);
                    }
				}
            }
		}
	}

    void UpdateAnimatorVars()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
    }

    IEnumerator Jump()
    {
		UpdateAnimatorVars();
        animator.CrossFade("jump", 0.1f);
        yield return new WaitForSeconds(2.1f);
        isJumping = false;
        UpdateAnimatorVars();
        animator.CrossFade("idle", 0.1f);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float groundRadius;
    [SerializeField] Vector3 groundOffset;
    [SerializeField] LayerMask groundMask;

    Rigidbody2D rigid;      // ������ٵ� �����ϴ� ����.
    int jumpCount;          // ���� ���� Ƚ��.

    bool isGrounded;        // ���� �ִ°�?
    bool isMove;            // ���� ����� ��������� �ߴ°�?
    float inputX;           // x�� �Է� ��.

    public bool IsGrounded => isGrounded;

    // ����Ƽ�� ���� �� ��� ������Ʈ�� �̺�Ʈ �Լ����� ��������Ʈ�� ����Ѵ�.
    // ����, ������ ���� �ش� �Լ����� ȣ���Ѵ�.
    // ��� ������� ȣ���� ���� ���¸� one loop(=1FRAME)�̶�� �ϸ�
    // �� �����Ӹ��� �� �ൿ�� �ݺ��Ѵ�.
    
    // ���� ���� �� ���ʿ� 1���� �Ҹ��� �ʱ�ȭ �Լ�.
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();        // ���� Rigidbody2D�� �˻��ؼ� �����Ѵ�.
        jumpCount = 1;                              // ���� Ƚ���� 1�� �ʱ�ȭ.

        // ���� ������Ʈ���Լ� Transform ������Ʈ�� �˻��� �����Ѵ�.
        Transform transform = GetComponent<Transform>();

        // ���� ���� 0,0�� �ְ� �������� 5��ŭ ���������ϴϱ�
        // (5,0)�� Transform�� position�� ��������!!
        // ������ �̷��� �� ������ �޶����� ������ �׻� (5,0)���� ����.
        // transform.position = new Vector3(5, 0);     

        // ���� ��ġ���� �������� 5 �������� 2��ŭ �����̴� �̵����� �����.
        // Vector3 movment = (Vector3.right * 5f) + (Vector3.up * 2f);       // ���� * �Ÿ� = �̵���.
        // transform.position += movment;                                    // ���� ��ġ���� �������� 5��ŭ ��������.

        // World ��ǥ(=���� ��ǥ)
        // = �׻� �������� �ʰ� ȸ������ �ʴ� ���带 �������� ��ŭ ������������..
        //   Vector3.right => �׻� (1,0)�� ���� ������.

        // Local ��ǥ(=��� ��ǥ)
        // = ���� �������� ��ŭ �������ְ� ��� ȸ�����ִ���..
        //   transform.right => ���� ȸ���ϴ� �Ϳ� ���� ���� ���Ͱ� �ٲ��.

        //transform.position += transform.right * 3f;
    }

    // �� �����Ӹ��� �Ҹ��� �Լ�.
    void LateUpdate()
    {
        // ���� üũ.
        // = ���� �浹 ������ ����� �� ���� �浹�ϴ� �浹ü�� ������ ���� �ִ�.
        Collider2D hit = Physics2D.OverlapCircle(transform.position + groundOffset, groundRadius, groundMask);
        isGrounded = hit != null;

        // ���鿡 �浹�߰�, �Ʒ��� �������� �ִ� ���. (=���� ����)
        if (isGrounded && rigid.velocity.y < -0.1f)
        {
            jumpCount = 1;
        }

        // �����ڰ� ��򰡷� ��������� �ߴٸ�..
        if (isMove)
        {
            rigid.velocity = new Vector2(inputX * moveSpeed, rigid.velocity.y);
            isMove = false;
        }
        // �����ڰ� ���̻� �������� �ʴ´ٸ�..
        else if(!isMove && inputX != 0)
        {
            rigid.velocity = new Vector2(0f, rigid.velocity.y);
            inputX = 0;
        }

        #region ��ǥ �̵� ���

        // ���� ���͸� ���Ҷ��� �׻� Normalize(=����ȭ)�� �������� �����ؾ��Ѵ�.
        // Vector3 direction = (transform.right * x) + (transform.up * y);
        // direction.Normalized();

        // ���� ��� �������� ������ �� ������� �������� ������ ��Ÿ������ ��Ģ�� ���� �� �ָ� �̵��ϰ� �ȴ�.
        // ���� ���� ����̶�� ���� ���͸� ���ϰ� �� ������ moveSpeed��ŭ �����δ�.

        // Time.deltaTime : ���� �����Ӻ��� ���� ������ ������ �ð� ����. (�� 0.000032f)
        // �ش� ���� ���ϴ� ������ ������ ���̿� ���� �ӵ� ���̸� �����ϱ� �����̴�.
        // transform.position += direction * moveSpeed * Time.deltaTime;

        #endregion
    }

    // Collider : �浹ü, ���� ����� �浹 üũ�� ���ÿ� �Ѵ�.
    // Trigger  : ����, �浹 üũ�� �Ѵ�. (���� �̺�Ʈ Ʈ������ ������ �Ѵ�.)

    public void Move(float x)
    {
        inputX = x;
        isMove = true;
    }
    public bool Jump()
    {
        // GetKeyDown   : �ش� Ű�� ���� �� ���� 1ȸ.
        // GetKeyUp     : �ش� Ű�� �� �� ���� 1ȸ.
        // GetKey       : �ش� Ű�� ������ ���� �� ���.
        if (jumpCount > 0)
        {
            // ForceMode2D.Force    : �������� ��.
            // ForceMode2D.Impulse  : �ѹ��� �վ��� ������ ��.

            // �����ϱ� ������ ���� y�� �ӵ��� 0���� �����Ѵ�.
            rigid.velocity = new Vector2(rigid.velocity.x, 0f);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            jumpCount -= 1;
            return true;
        }

        return false;
    }
    public void Throw(bool isLeft)
    {
        inputX = 0;
        rigid.velocity = Vector2.zero;  // ������ �� ������ �ִ� ���� �����Ѵ�.

        // ���� ���, ���� ������� ������ ����Ѵ�.
        Vector2 direction = Vector2.up + (isLeft ? Vector2.left : Vector2.right);
        rigid.AddForce(direction * 5f, ForceMode2D.Impulse);
    }

    /*
    // ��� �浹ü�� �浹�� �� ����.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Enter : {collision.gameObject.name}");
    }
    // ��� �浹ü�� �浹�ϰ� �ִ� ���� ���.
    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log($"Stay : {collision.gameObject.name}");
    }
    // ��� �浹ü�� �浹�ϰ� �־��µ� �����Ǵ� �� ����.
    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log($"Exit : {collision.gameObject.name}");
    }

    // ��� Ʈ���ſ� �浹�ϴ� �� ����.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Trigger Enter : {collision.name}");
    }
    */

    // Collider  : �浹ü, ��ġ, ũ��, Ʈ���ſ��� ���� ���ԵǾ� �ִ� �ڷ���.
    // Collision : �浹, �ݶ��̴��� ���� �浹�� ����, ��ġ, �ӵ�, �� ���� ���ԵǾ��ִ� �ڷ���.

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + groundOffset, groundRadius);
    }

}
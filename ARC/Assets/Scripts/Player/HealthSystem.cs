using UnityEngine;
using Photon.Pun;

public class HealthSystem : MonoBehaviourPun
{
    [Header("ü�� ����")]
    public float maxHealth = 100f;
    public float currentHealth;

    private bool isDead = false;

    public delegate void OnHealthChanged(float current, float max);
    public event OnHealthChanged onHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        NotifyHealthChanged();
    }

    [PunRPC]
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0f, currentHealth);
        NotifyHealthChanged();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    [PunRPC]
    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        NotifyHealthChanged();
    }

    void Die()
    {
        isDead = true;
        // �׾��� �� ó��: �ٿ�, ������ ���, UI ��
        Debug.Log($"{gameObject.name} ���");

        // ����: ���� ��Ȱ��ȭ
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveRPCs(photonView);
            GetComponent<PlayerController>().enabled = false;
        }
    }

    public void ApplyDamage(float amount)
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(TakeDamage), RpcTarget.AllBuffered, amount);
        }
    }

    public void ApplyHeal(float amount)
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(Heal), RpcTarget.AllBuffered, amount);
        }
    }

    void NotifyHealthChanged()
    {
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}

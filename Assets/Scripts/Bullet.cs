using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision ObjectWeHit)
    {
        if (ObjectWeHit.gameObject.CompareTag("Target"))
        {
            print("hit " + ObjectWeHit.gameObject.name + " !");
            CreateBulletImpactEffect(ObjectWeHit);
            Destroy(gameObject);
        }

        if (ObjectWeHit.gameObject.CompareTag("Wall"))
        {
            print("Kena Tembok Bro! ");
            CreateBulletImpactEffect(ObjectWeHit);
            Destroy(gameObject);
        }

        if (ObjectWeHit.gameObject.CompareTag("Beer"))
        {
            print("Kena Botol Haram!");
            ObjectWeHit.gameObject.GetComponent<BeerBottle>().Shatter();
        }
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];

        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
            );

        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}

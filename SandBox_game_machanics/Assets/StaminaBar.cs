using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    // Déclaration des variable 
    public Slider staminaBar;
    public int currentStamina;
    public static StaminaBar instance;
    private WaitForSeconds waitForOne = new WaitForSeconds(0.1f);
    private Coroutine reload;

    // Valeurs exposées
    [SerializeField]
    int maxStamina = 100;

    [SerializeField]
    int timeBeforeReload = 2;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;

    }

    public Boolean UseStamina(int need)
    {
        if (currentStamina >= need)
        {
            currentStamina -= need;
            staminaBar.value = currentStamina;

            if (reload != null)
            {
                StopCoroutine(reload);
            }

            reload = StartCoroutine(reloadStamina());

            return true;
        }
        else
        {
            return false;
        }

    }

    private IEnumerator reloadStamina()
    {
        yield return new WaitForSeconds(timeBeforeReload);
        while (currentStamina < maxStamina)
        {
            currentStamina += maxStamina / 100; //We gain 1% 
            staminaBar.value = currentStamina;
            yield return waitForOne;
        }

        reload = null;
    }


}

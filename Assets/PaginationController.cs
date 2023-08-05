using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaginationController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI paginatedText;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button prevBtn;
    [SerializeField] private TextMeshProUGUI pageCountText;
    [SerializeField] private AudioSource audioSource;
    private int pageCount;

    private void OnEnable()
    {
        UpdateControls();
    }

    void UpdateControls()
    {
        pageCount = paginatedText.textInfo.pageCount;
        if (pageCount <= 1)
        {
            pageCountText.gameObject.SetActive(false);
            prevBtn.gameObject.SetActive(false);
            nextBtn.gameObject.SetActive(false);
        }
        else
        {
            pageCountText.gameObject.SetActive(true);
            pageCountText.text = paginatedText.pageToDisplay + " <font=\"Itim-Regular SDF\">/ " + pageCount + "</font>";

            if (paginatedText.pageToDisplay > 1)
            {
                prevBtn.gameObject.SetActive(true);
            }
            else
            {
                prevBtn.gameObject.SetActive(false);
            }

            if (paginatedText.pageToDisplay < pageCount)
            {
                nextBtn.gameObject.SetActive(true);
            }
            else
            {
                nextBtn.gameObject.SetActive(false);
            }
        }
    }

    public void ChangePage(int increment)
    {
        audioSource.Stop();
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();

        int newPage = paginatedText.pageToDisplay + increment;
        if (newPage >= 1 && newPage <= pageCount)
        {
            paginatedText.pageToDisplay = newPage;
            UpdateControls();
        }
    }
}

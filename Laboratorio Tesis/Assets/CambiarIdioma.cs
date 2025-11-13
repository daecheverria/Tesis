using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class CambiarIdioma : MonoBehaviour
{
    [Tooltip("Busca locales cuyo código comience por este prefijo (ej. 'es' → 'es-ES')")]
    public string spanishCode = "es";

    [Tooltip("Busca locales cuyo código comience por este prefijo (ej. 'en' → 'en-US')")]
    public string englishCode = "en";

    // Llamar desde UI / evento XR: SetSpanish() o SetEnglish()
    public void SetSpanish() => SetLocaleByCode(spanishCode);
    public void SetEnglish() => SetLocaleByCode(englishCode);

    private void SetLocaleByCode(string code)
    {
        // Si la inicialización de Localización aún no ha terminado, espera al completion
        if (!LocalizationSettings.InitializationOperation.IsDone)
        {
            LocalizationSettings.InitializationOperation.Completed += _ => ApplyLocale(code);
            return;
        }

        ApplyLocale(code);
    }

    private void ApplyLocale(string code)
    {
        var locales = LocalizationSettings.AvailableLocales?.Locales;
        if (locales == null || locales.Count == 0)
        {
            Debug.LogWarning("CambiarIdioma: no hay locales disponibles en LocalizationSettings.");
            return;
        }

        // Buscar primer locale cuyo identificador empiece por el código pedido
        var locale = locales.FirstOrDefault(l => l?.Identifier != null && l.Identifier.Code.StartsWith(code));
        if (locale == null)
        {
            Debug.LogWarning($"CambiarIdioma: no se encontró locale que comience por '{code}'.");
            return;
        }

        LocalizationSettings.SelectedLocale = locale;
        Debug.Log($"CambiarIdioma: locale cambiado a {locale.Identifier.Code}");
    }
}

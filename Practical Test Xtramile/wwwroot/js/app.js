/**
 * Xtramile Weather & Telemetry Portal - Client Application
 * Implemented with design-taste-frontend-v1 directives:
 * - Anti-Emoji Policy (Clean SVG vectors with standardized 1.75 stroke-width)
 * - Spring micro-interaction states
 * - Robust error handling and state persistence
 */

document.addEventListener('DOMContentLoaded', () => {
  // DOM Elements
  const countrySelect = document.getElementById('countrySelect');
  const citySelect = document.getElementById('citySelect');
  
  const weatherEmptyState = document.getElementById('weatherEmptyState');
  const weatherLoadingState = document.getElementById('weatherLoadingState');
  const weatherContent = document.getElementById('weatherContent');

  // Weather Card Elements
  const countryCodeBadge = document.getElementById('countryCodeBadge');
  const countryNameText = document.getElementById('countryNameText');
  const cityNameText = document.getElementById('cityNameText');
  const timeUtcText = document.getElementById('timeUtcText');
  const conditionIcon = document.getElementById('conditionIcon');
  const conditionText = document.getElementById('conditionText');

  const tempCelsius = document.getElementById('tempCelsius');
  const tempFahrenheit = document.getElementById('tempFahrenheit');
  const dewPointCelsius = document.getElementById('dewPointCelsius');
  const dewPointFahrenheit = document.getElementById('dewPointFahrenheit');

  const windSpeed = document.getElementById('windSpeed');
  const windDirection = document.getElementById('windDirection');
  const humidityVal = document.getElementById('humidityVal');
  const pressureVal = document.getElementById('pressureVal');
  const visibilityVal = document.getElementById('visibilityVal');
  const visibilityMeters = document.getElementById('visibilityMeters');

  // Note & Favorite Elements
  const btnFavorite = document.getElementById('btnFavorite');
  const btnFavoriteText = document.getElementById('btnFavoriteText');
  const noteForm = document.getElementById('noteForm');
  const noteInput = document.getElementById('noteInput');
  const btnSubmitNote = document.getElementById('btnSubmitNote');
  const charCounter = document.getElementById('charCounter');
  const statusToast = document.getElementById('statusToast');
  const toastTitle = document.getElementById('toastTitle');
  const toastMessage = document.getElementById('toastMessage');

  // Current State
  let currentCountry = null;
  let currentCity = null;
  let currentWeather = null;
  let toastTimeout = null;

  // 1. Initialize: Fetch Countries
  loadCountries();

  async function loadCountries() {
    try {
      const response = await fetch('/api/countries');
      if (!response.ok) throw new Error('Failed to load supported countries.');

      const countries = await response.json();
      countrySelect.innerHTML = '<option value="" disabled selected>Select a country</option>';

      countries.forEach(country => {
        const opt = document.createElement('option');
        opt.value = country.code;
        opt.textContent = `${country.name} (${country.code})`;
        opt.dataset.countryName = country.name;
        opt.dataset.countryId = country.id;
        countrySelect.appendChild(opt);
      });
    } catch (err) {
      showToast('Connection Error', err.message, 'error');
      countrySelect.innerHTML = '<option value="" disabled selected>Failed to load countries</option>';
    }
  }

  // 2. Event: Country Selection Changed
  countrySelect.addEventListener('change', async () => {
    const selectedOpt = countrySelect.options[countrySelect.selectedIndex];
    const countryCode = selectedOpt.value;
    const countryName = selectedOpt.dataset.countryName;

    currentCountry = { code: countryCode, name: countryName };
    currentCity = null;
    currentWeather = null;

    // Reset UI state
    citySelect.disabled = true;
    citySelect.innerHTML = '<option value="" disabled selected>Loading cities...</option>';
    btnFavorite.disabled = true;
    btnFavorite.classList.remove('favorited');
    btnFavoriteText.textContent = 'Add to Favorites';
    noteInput.disabled = true;
    noteInput.value = '';
    charCounter.textContent = '0';
    btnSubmitNote.disabled = true;

    showEmptyState();

    try {
      const response = await fetch(`/api/countries/${encodeURIComponent(countryCode)}/cities`);
      if (!response.ok) throw new Error('Failed to load cities for the selected country.');

      const cities = await response.json();
      citySelect.innerHTML = '<option value="" disabled selected>Select a city</option>';

      cities.forEach(city => {
        const opt = document.createElement('option');
        opt.value = city.name;
        opt.textContent = city.name;
        opt.dataset.cityId = city.id;
        citySelect.appendChild(opt);
      });

      citySelect.disabled = false;
    } catch (err) {
      showToast('Error', err.message, 'error');
      citySelect.innerHTML = '<option value="" disabled selected>Error loading cities</option>';
    }
  });

  // 3. Event: City Selection Changed
  citySelect.addEventListener('change', async () => {
    const cityName = citySelect.value;
    if (!cityName) return;

    currentCity = cityName;
    btnFavorite.classList.remove('favorited');
    btnFavoriteText.textContent = 'Add to Favorites';

    showLoadingState();

    try {
      const response = await fetch(`/api/weather/${encodeURIComponent(cityName)}`);
      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.detail || errorData?.title || 'Failed to fetch weather telemetry.');
      }

      currentWeather = await response.json();
      renderWeatherData(currentWeather);

      // Enable actions
      btnFavorite.disabled = false;
      noteInput.disabled = false;
      btnSubmitNote.disabled = false;
    } catch (err) {
      showToast('Weather Error', err.message, 'error');
      showEmptyState();
    }
  });

  // 4. Render Weather Telemetry
  function renderWeatherData(data) {
    countryCodeBadge.textContent = data.location.countryCode || currentCountry?.code || 'GL';
    countryNameText.textContent = data.location.country || currentCountry?.name || 'Global';
    cityNameText.textContent = data.location.city || currentCity;
    
    const obsDate = new Date(data.timeUtc);
    timeUtcText.textContent = obsDate.toUTCString();

    // Condition and Standardized 1.75 Stroke Vector SVG (Anti-Emoji Directive)
    conditionText.textContent = data.skyConditions;
    conditionIcon.innerHTML = getWeatherIconSvg(data.skyConditions);

    // Temperature & Dew Point
    tempCelsius.textContent = data.temperature.celsius.toFixed(1);
    tempFahrenheit.textContent = data.temperature.fahrenheit.toFixed(1);
    dewPointCelsius.textContent = data.dewPoint.celsius.toFixed(1);
    dewPointFahrenheit.textContent = data.dewPoint.fahrenheit.toFixed(1);

    // Metrics
    windSpeed.textContent = `${data.wind.speedMph.toFixed(1)} mph`;
    windDirection.textContent = `${data.wind.directionDegrees}° ${data.wind.directionCardinal}`;
    humidityVal.textContent = `${data.relativeHumidityPercent}%`;
    pressureVal.textContent = `${data.pressureHpa.toFixed(1)} hPa`;

    const visKm = (data.visibilityMeters / 1000).toFixed(1);
    visibilityVal.textContent = `${visKm} km`;
    visibilityMeters.textContent = `${data.visibilityMeters.toLocaleString()} meters`;

    // Show Card Content with Waterfall Animation Cascade
    weatherEmptyState.classList.add('hidden');
    weatherLoadingState.classList.add('hidden');
    weatherContent.classList.remove('hidden');
  }

  // Vector SVG Glyph Generator for Weather Conditions (Standardized 1.75 stroke-width)
  function getWeatherIconSvg(condition) {
    const c = (condition || '').toLowerCase();
    
    if (c.includes('sun') || c.includes('clear')) {
      return `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round">
        <circle cx="12" cy="12" r="5"></circle>
        <line x1="12" y1="1" x2="12" y2="3"></line>
        <line x1="12" y1="21" x2="12" y2="23"></line>
        <line x1="4.22" y1="4.22" x2="5.64" y2="5.64"></line>
        <line x1="18.36" y1="18.36" x2="19.78" y2="19.78"></line>
        <line x1="1" y1="12" x2="3" y2="12"></line>
        <line x1="21" y1="12" x2="23" y2="12"></line>
        <line x1="4.22" y1="19.78" x2="5.64" y2="18.36"></line>
        <line x1="18.36" y1="5.64" x2="19.78" y2="4.22"></line>
      </svg>`;
    }
    if (c.includes('cloud') || c.includes('overcast')) {
      return `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round">
        <path d="M18 10h-1.26A8 8 0 1 0 9 20h9a5 5 0 0 0 0-10z"></path>
      </svg>`;
    }
    if (c.includes('rain') || c.includes('drizzle')) {
      return `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round">
        <line x1="16" y1="13" x2="16" y2="21"></line>
        <line x1="8" y1="13" x2="8" y2="21"></line>
        <line x1="12" y1="15" x2="12" y2="23"></line>
        <path d="M20 16.58A5 5 0 0 0 18 7h-1.26A8 8 0 1 0 4 15.25"></path>
      </svg>`;
    }
    if (c.includes('thunder') || c.includes('storm')) {
      return `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round">
        <path d="M19 16.9A5 5 0 0 0 18 7h-1.26a8 8 0 1 0-11.62 9"></path>
        <polyline points="13 11 9 17 15 17 11 23"></polyline>
      </svg>`;
    }
    if (c.includes('snow')) {
      return `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round">
        <path d="M20 17.58A5 5 0 0 0 18 8h-1.26A8 8 0 1 0 4 16.25"></path>
        <line x1="8" y1="16" x2="8.01" y2="16"></line>
        <line x1="12" y1="18" x2="12.01" y2="18"></line>
        <line x1="16" y1="16" x2="16.01" y2="16"></line>
      </svg>`;
    }
    if (c.includes('fog') || c.includes('mist') || c.includes('haze')) {
      return `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round">
        <line x1="3" y1="8" x2="21" y2="8"></line>
        <line x1="3" y1="12" x2="21" y2="12"></line>
        <line x1="5" y1="16" x2="19" y2="16"></line>
        <line x1="7" y1="20" x2="17" y2="20"></line>
      </svg>`;
    }
    
    // Default Atmosphere / Sun-Cloud
    return `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round">
      <circle cx="12" cy="12" r="4"></circle>
      <path d="M12 2v2"></path>
      <path d="M12 20v2"></path>
      <path d="m4.93 4.93 1.41 1.41"></path>
      <path d="m17.66 17.66 1.41 1.41"></path>
      <path d="M2 12h2"></path>
      <path d="M20 12h2"></path>
    </svg>`;
  }

  // 5. Note Input Character Counter
  noteInput.addEventListener('input', () => {
    charCounter.textContent = noteInput.value.length;
  });

  // 6. Action: Save Weather Note
  noteForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    if (!currentCity || !noteInput.value.trim()) return;

    btnSubmitNote.disabled = true;

    const payload = {
      cityName: currentCity,
      note: noteInput.value.trim(),
      temperatureCelsius: currentWeather ? currentWeather.temperature.celsius : null,
      skyConditions: currentWeather ? currentWeather.skyConditions : null
    };

    try {
      const response = await fetch('/api/weather/notes', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.detail || 'Failed to save note.');
      }

      const result = await response.json();
      showToast('Note Persisted', `Note recorded with ID ${result.id.substring(0, 8)} for ${result.cityName}`, 'success');
      noteInput.value = '';
      charCounter.textContent = '0';
    } catch (err) {
      showToast('Submission Failed', err.message, 'error');
    } finally {
      btnSubmitNote.disabled = false;
    }
  });

  // 7. Action: Add to Favorites
  btnFavorite.addEventListener('click', async () => {
    if (!currentCity || !currentCountry) return;

    btnFavorite.disabled = true;

    const payload = {
      cityName: currentCity,
      countryCode: currentCountry.code
    };

    try {
      const response = await fetch('/api/cities/favorites', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.detail || 'Failed to add favorite city.');
      }

      const result = await response.json();
      btnFavorite.classList.add('favorited');
      btnFavoriteText.textContent = 'Saved in Favorites';
      showToast('Favorite Saved', `${result.cityName} (${result.countryCode}) added to favorites`, 'success');
    } catch (err) {
      showToast('Action Failed', err.message, 'error');
    } finally {
      btnFavorite.disabled = false;
    }
  });

  // Helper State Transitions
  function showEmptyState() {
    weatherEmptyState.classList.remove('hidden');
    weatherLoadingState.classList.add('hidden');
    weatherContent.classList.add('hidden');
  }

  function showLoadingState() {
    weatherEmptyState.classList.add('hidden');
    weatherLoadingState.classList.remove('hidden');
    weatherContent.classList.add('hidden');
  }

  function showToast(title, message, type = 'success') {
    if (toastTimeout) clearTimeout(toastTimeout);

    toastTitle.textContent = title;
    toastMessage.textContent = message;
    statusToast.className = `status-toast ${type}`;

    toastTimeout = setTimeout(() => {
      statusToast.className = 'status-toast hidden';
    }, 4500);
  }
});

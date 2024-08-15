import React, { useEffect, useState } from 'react';
import { Bar } from 'react-chartjs-2';
import 'chart.js/auto';
import { Chart as ChartJS, Title, Tooltip, Legend, LineElement, PointElement, LinearScale, CategoryScale } from 'chart.js';

ChartJS.register(Title, Tooltip, Legend, LineElement, PointElement, LinearScale, CategoryScale);

interface WeatherRecord {
  timestamp: string;
  country: string;
  city: string;
  minTemp: number;
  maxTemp: number;
  currentTemp: number;
  humidity: number;
}

const WeatherChart: React.FC = () => {
  const [weatherData, setWeatherData] = useState<WeatherRecord[]>([]);
  const [error, setError] = useState<string | null>(null);

  // Function to fetch the latest weather data
  const fetchWeatherData = async () => {
    try {
      const response = await fetch('https://localhost:5000/api/weather/latest'); // Use the correct URL
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      const data: WeatherRecord[] = await response.json();
      setWeatherData(data);
      setError(null); // Clear any previous error
    } catch (error: any) {
      console.error('Error fetching weather data:', error);
      setError('Failed to fetch weather data');
    }
  };

  useEffect(() => {
    // Initial fetch
    fetchWeatherData();

    // Set up polling
    const intervalId = setInterval(fetchWeatherData, 60000); // Poll every 60 seconds

    // Clean up interval on component unmount
    return () => clearInterval(intervalId);
  }, []);

  // Format x-axis labels to include city and country
  const labels = weatherData.map((data) => `${data.city}, ${data.country}`);

  const chartData = {
    labels: labels,
    datasets: [
      {
        label: 'Min Temp',
        data: weatherData.map((data) => ({ x: `${data.city}, ${data.country}`, y: Math.round((data.minTemp - 273.15)*100)/100, timestamp: data.timestamp })),
        borderColor: 'blue',
        backgroundColor: 'lightblue',
      },
      {
        label: 'Max Temp',
        data: weatherData.map((data) => ({ x: `${data.city}, ${data.country}`, y: Math.round((data.maxTemp - 273.15)*100)/100, timestamp: data.timestamp })),
        borderColor: 'red',
        backgroundColor: 'lightcoral',
      },
    ],
  };

  const options = {
    plugins: {
      tooltip: {
        callbacks: {
          title: function (tooltipItems: any[]) {
            // Return the timestamp as part of the tooltip title
            return `Timestamp: ${new Date(tooltipItems[0].raw.timestamp).toUTCString()}`;
          },
          label: function (tooltipItem: any) {
            // Return the temperature and label
            const { dataset } = tooltipItem;
            const value = tooltipItem.raw.y;
            const label = dataset.label;
            return `${label}: ${value}°C`;
          }
        },
      },
    },
    scales: {
      x: {
        title: {
          display: true,
          text: 'City, Country'
        },
      },
      y: {
        title: {
          display: true,
          text: 'Temperature (°C)'
        },
      },
    },
  };

  return (
    <div>
      <h2>Latest Weather Data</h2>
      {error && <p>{error}</p>}
      <Bar data={chartData} options={options} />
    </div>
  );
};

export default WeatherChart;

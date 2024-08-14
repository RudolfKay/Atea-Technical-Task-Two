import React, { useEffect, useState } from 'react';
import { Line } from 'react-chartjs-2';
import 'chart.js/auto';

interface WeatherData {
  timestamp: string;
  country: string;
  city: string;
  minTemp: number;
  maxTemp: number;
}

const WeatherChart: React.FC = () => {
  const [weatherData, setWeatherData] = useState<WeatherData[]>([]);
  const [error, setError] = useState<string | null>(null);

  // Function to fetch the latest weather data
  const fetchWeatherData = async () => {
    try {
      const response = await fetch('https://localhost:5000/api/weather/latest'); // Use the correct URL
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      const data: WeatherData[] = await response.json();
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

  const chartData = {
    labels: weatherData.map((data) => `${data.city}, ${data.country}`),
    datasets: [
      {
        label: 'Min Temp',
        data: weatherData.map((data) => data.minTemp),
        borderColor: 'blue',
        backgroundColor: 'lightblue',
        fill: false,
      },
      {
        label: 'Max Temp',
        data: weatherData.map((data) => data.maxTemp),
        borderColor: 'red',
        backgroundColor: 'lightcoral',
        fill: false,
      },
    ],
  };

  return (
    <div>
      <h2>Latest Weather Data</h2>
      {error && <p>{error}</p>}
      <Line data={chartData} />
    </div>
  );
};

export default WeatherChart;

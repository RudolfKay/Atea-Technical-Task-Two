import WeatherChart from './WeatherChart';
import React from 'react';

const App: React.FC = () => {
  return (
    <div className="App">
      <header className="App-header">
        <h1>Weather Dashboard</h1>
        <WeatherChart />
      </header>
    </div>
  );
}

export default App;

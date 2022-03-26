import React from 'react';
import ReactDOM from 'react-dom';

import './assets/styles/style.css';
import Player from './components/Player';

ReactDOM.render(
  React.createElement(Player),
  document.getElementById('root'),
);

// Check if hot reloading is enable. If it is, changes won't reload the page.
// This is related to webpack-dev-server and works on development only.
if (module.hot) {
  module.hot.accept();
}

import React, { Component } from "react";
import AirConsole from "air-console";
import IntroView from "./IntroView";
import PlayView from "./PlayView";

const views = {
  PlayView,
  IntroView
};

class App extends Component {
  constructor(props) {
    super(props);

    this.state = {
      currentView: "IntroView"
    };

    props.airconsole.onMessage = (id, data) => {
      switch (data.action) {
        case "gameState":
          this.setState(prevState => ({
            currentView: data.view
          }));
          break;
      }
    };
  }

  render() {
    const { airconsole } = this.props;
    const View = views[this.state.currentView];

    return <View airconsole={airconsole} />;
  }
}

export default App;

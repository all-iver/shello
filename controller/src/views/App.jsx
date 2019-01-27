import React, { Component } from "react";
import AirConsole from "air-console";
import IntroView from "./IntroView";
import PlayView from "./PlayView";
import WaitingView from "./WaitingView";

const views = {
  PlayView,
  IntroView,
  WaitingView
};

class App extends Component {
  constructor(props) {
    super(props);

    this.state = {
      currentView: "IntroView",
      body: "green",
      number: "?",
      turtleHidden: false
    };

    props.airconsole.onMessage = (id, data) => {
      switch (data.action) {
        case "gameState":
          this.setState(prevState => ({
            currentView: data.view
          }));
          break;
        case "turtle":
          this.setState(prevState => ({
            body: data.color || prevState.body,
            number: data.number || prevState.number,
            turtleHidden: false
          }));
          break;
      }
    };
  }

  render() {
    const { body, number, turtleHidden } = this.state;
    const { airconsole } = this.props;
    const View = views[this.state.currentView];

    return (
      <View
        airconsole={airconsole}
        body={body}
        number={number}
        turtleHidden={turtleHidden}
        hideTurtle={() => {
          this.setState({
            turtleHidden: true
          });
        }}
      />
    );
  }
}

export default App;

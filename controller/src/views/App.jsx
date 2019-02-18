import React, { Component } from "react";
import AirConsole from "air-console";
import IntroView from "./IntroView";
import PlayView from "./PlayView";
import WaitingView from "./WaitingView";
import VictoryView from "./VictoryView";
import LoadingView from "./LoadingView";

const views = {
  PlayView,
  IntroView,
  WaitingView,
  VictoryView,
  LoadingView
};

class App extends Component {
  constructor(props) {
    super(props);

    this.state = {
      currentView: "PlayView",
      body: "green",
      number: "?",
      turtleHidden: true,
      showBow: false,
      loaded: false
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
            turtleHidden: false,
            showBow: data.showBow === "true" ? true : false,
            time: parseFloat(data.time),
            place: parseInt(data.place)
          }));
          break;
      }
    };
  }

  render() {
    const {
      body,
      number,
      turtleHidden,
      showBow,
      time,
      place,
      loaded
    } = this.state;
    const { airconsole } = this.props;
    const View = loaded ? views[this.state.currentView] : LoadingView;

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
        showBow={showBow}
        time={time}
        place={place}
        setLoaded={() =>
          this.setState({
            loaded: true
          })
        }
      />
    );
  }
}

export default App;

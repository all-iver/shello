import React, { Component } from "react";
import Flipper from "../components/Flipper";
import Turtle from "../components/Turtle";

class PlayView extends Component {
  constructor(props) {
    super(props);
    this.state = {
      previousInput: undefined
    };

    props.airconsole.vibrate(500);
  }

  sendMessage(swipe) {
    const { airconsole } = this.props;
    const { previousInput } = this.state;

    if (previousInput === swipe.action) {
      airconsole.message(AirConsole.SCREEN, {
        action: swipe.action
      });
      // console.log(swipe.action);
    } else {
      airconsole.message(AirConsole.SCREEN, {
        action: "swipeStraightEnd"
      });
      // console.log("straight");
    }

    this.setState({
      previousInput: swipe.action
    });
  }

  render() {
    const { airconsole, body, number, showBow } = this.props;

    return (
      <div id="playView">
        <div id="playViewTurtles">
          <Turtle
            showSwipeArrows={true}
            body={body}
            number={number}
            showBow={showBow}
          />
        </div>
        <div id="flippers">
          <Flipper
            airconsole={airconsole}
            side="Left"
            sendMessage={swipe => this.sendMessage(swipe)}
          />
          <Flipper
            airconsole={airconsole}
            side="Right"
            sendMessage={swipe => this.sendMessage(swipe)}
          />
        </div>
        <p>Swipe to the rhythm to move faster!</p>
      </div>
    );
  }
}

export default PlayView;

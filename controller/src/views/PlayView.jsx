import React, { Component } from "react";
import Flipper from "../components/Flipper";
import Turtle from "../components/Turtle";

class PlayView extends Component {
  constructor(props) {
    super(props);

    props.airconsole.vibrate(500);
  }

  render() {
    const { airconsole, body, number, showBow } = this.props;

    return (
      <div id="playView">
        <p>Swipe to move!</p>
        <div id="playViewTurtles">
          <Turtle
            showSwipeArrows={true}
            body={body}
            number={number}
            showBow={showBow}
          />
        </div>
        <div id="flippers">
          <Flipper airconsole={airconsole} side="Left" />
          <Flipper airconsole={airconsole} side="Right" />
        </div>
      </div>
    );
  }
}

export default PlayView;

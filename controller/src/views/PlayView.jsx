import React, { Component } from "react";
import Flipper from "../components/Flipper";
import Turtle from "../components/Turtle";

class PlayView extends Component {
  render() {
    const { airconsole, body, number } = this.props;

    return (
      <div id="playView">
        <div id="playViewTurtles">
          <Turtle showSwipeArrows={true} body={body} number={number} />
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

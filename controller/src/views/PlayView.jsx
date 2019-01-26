import React, { Component } from "react";
import Flipper from "../components/Flipper";

class PlayView extends Component {
  render() {
    const { airconsole } = this.props;

    return (
      <div id="playView">
        <Flipper airconsole={airconsole} side="Left" />
        <Flipper airconsole={airconsole} side="Right" />
      </div>
    );
  }
}

export default PlayView;

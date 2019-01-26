import React, { Component } from "react";
import Flipper from "../components/Flipper";

class PlayView extends Component {
  render() {
    this.props = { airconsole };

    return (
      <div>
        <Flipper airconsole={airconsole} side="Left" />
        <Flipper airconsole={airconsole} side="Right" />
      </div>
    );
  }
}

export default PlayView;

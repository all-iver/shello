import React, { Component } from "react";

import turtleBody from "../../assets/images/Turtle_body.png";

class Egg extends Component {
  render() {
    return (
      <div className="turtle">
        <img src={turtleBody} />
      </div>
    );
  }
}

export default Egg;

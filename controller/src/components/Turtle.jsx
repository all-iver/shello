import React, { Component } from "react";

import turtleBody from "../../assets/images/turtle_body_teal.png";
import turtleLegsLeft from "../../assets/images/Turtle_legs_left 1.png";
import turtleLegsRight from "../../assets/images/Turtle_legs_right 1.png";

class Egg extends Component {
  render() {
    return (
      <div className="turtle">
        <img src={turtleBody} className="body" />
        <img src={turtleLegsLeft} className="left-legs" />
        <img src={turtleLegsRight} className="right-legs" />
      </div>
    );
  }
}

export default Egg;

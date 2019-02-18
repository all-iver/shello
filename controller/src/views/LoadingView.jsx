import React, { Component } from "react";

import turtleBlack1 from "../../assets/images/bodies/turtle_black_1.png";
import turtleBlack2 from "../../assets/images/bodies/turtle_black_2.png";
import turtleBlue1 from "../../assets/images/bodies/turtle_blue_1.png";
import turtleBlue2 from "../../assets/images/bodies/turtle_blue_2.png";
import turtleBronze1 from "../../assets/images/bodies/turtle_bronze_1.png";
import turtleBronze2 from "../../assets/images/bodies/turtle_bronze_2.png";
import turtleGold1 from "../../assets/images/bodies/turtle_gold_1.png";
import turtleGold2 from "../../assets/images/bodies/turtle_gold_2.png";
import turtleGray1 from "../../assets/images/bodies/turtle_gray_1.png";
import turtleGray2 from "../../assets/images/bodies/turtle_gray_2.png";
import turtleGreen1 from "../../assets/images/bodies/turtle_green_1.png";
import turtleGreen2 from "../../assets/images/bodies/turtle_green_2.png";
import turtlePink1 from "../../assets/images/bodies/turtle_pink_1.png";
import turtlePink2 from "../../assets/images/bodies/turtle_pink_2.png";
import turtlePurple1 from "../../assets/images/bodies/turtle_purple_1.png";
import turtlePurple2 from "../../assets/images/bodies/turtle_purple_2.png";
import turtleRed1 from "../../assets/images/bodies/turtle_red_1.png";
import turtleRed2 from "../../assets/images/bodies/turtle_red_2.png";
import turtleTeal1 from "../../assets/images/bodies/turtle_teal_1.png";
import turtleTeal2 from "../../assets/images/bodies/turtle_teal_2.png";
import egg1 from "../../assets/images/egg_1.png";
import egg2 from "../../assets/images/egg_2.png";
import egg3 from "../../assets/images/egg_3.png";
import egg4 from "../../assets/images/egg_4.png";
import eggShadow from "../../assets/images/egg_shadow.png";
import eggShatter from "../../assets/images/egg_shatter.png";
import sand from "../../assets/images/sand.png";
import turtleLegsLeft1 from "../../assets/images/turtle_legs_left_1.png";
import turtleLegsLeft2 from "../../assets/images/turtle_legs_left_2.png";
import turtleLegsRight1 from "../../assets/images/turtle_legs_right_1.png";
import turtleLegsRight2 from "../../assets/images/turtle_legs_right_2.png";
import turtleWinnerBow from "../../assets/images/turtle_winner_bow.png";

class LoadingView extends Component {
  constructor(props) {
    super(props);

    this.state = {
      filesLoaded: 0
    };

    this.images = [
      turtleBlack1,
      turtleBlack2,
      turtleBlue1,
      turtleBlue2,
      turtleBronze1,
      turtleBronze2,
      turtleGold1,
      turtleGold2,
      turtleGray1,
      turtleGray2,
      turtleGreen1,
      turtleGreen2,
      turtlePink1,
      turtlePink2,
      turtlePurple1,
      turtlePurple2,
      turtleRed1,
      turtleRed2,
      turtleTeal1,
      turtleTeal2,
      egg1,
      egg2,
      egg3,
      egg4,
      eggShadow,
      eggShatter,
      sand,
      turtleLegsLeft1,
      turtleLegsLeft2,
      turtleLegsRight1,
      turtleLegsRight2,
      turtleWinnerBow
    ];

    const imagePromises = this.images.map(
      image =>
        new Promise(resolve => {
          const img = new Image();
          img.onload = () => {
            this.setState(prevState => ({
              filesLoaded: prevState.filesLoaded + 1
            }));
            resolve();
          };
          img.src = image;
        })
    );

    const { setCurrentView } = props;

    Promise.all(imagePromises)
      .then(() => {
        console.log("all images loaded");
        setCurrentView("IntroView");
      })
      .catch(() => {
        console.log("error loading images");
      });

    // var images = ["button_1.png", "button_2.png", "character_1.png"];
    // for (var i = 0; i < images.length; i++) {
    //   var img = new Image();
    //   img.onload = () => {};
    //   img.src = "assets/images" + images[i];
    // }
  }

  render() {
    const { filesLoaded } = this.state;

    return (
      <div id="loadingView">
        <progress value={filesLoaded} max={this.images.length} />
      </div>
    );
  }
}

export default LoadingView;

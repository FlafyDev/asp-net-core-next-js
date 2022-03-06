import React, { useState } from 'react';
import styles from './useBackgroundTransitioner.module.css';

const useBackgroundTransitioner = (defaultBackground) => {
	const [backgrounds, setBackgrounds] = useState([]);

	const backgroundElement = (
		<div
			className={styles.background}
			style={{ background: defaultBackground }}
		>
			{
				backgrounds.map((back, i) => (
					i < backgrounds.length - 5 ? <></> : <div
						className={styles.backgroundManipulator}
						style={{ background: back }}
						key={i}
					></div>
				))
			}
		</div>
	);

	// This is leaking but cutting the array doesn't do good for the css animations.
	// Ended up leaving the backgrounds in the string array but added line 15 to not render them.
	const addBackground = (background) => {
		const startIndex = backgrounds.length - 5;
		const newBackgroundsArray = [...backgrounds, background]; // [...(backgrounds.slice(startIndex >= 0 ? startIndex : 0, backgrounds.length)), background];
		console.log(newBackgroundsArray)
		setBackgrounds(newBackgroundsArray)
	};

	return [backgroundElement, addBackground]
}

export default useBackgroundTransitioner;
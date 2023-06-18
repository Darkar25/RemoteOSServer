// by LizAinslie
// https://github.com/LizAinslie/mc-three
import {Matrix3,Vector3,BufferGeometry,Float32BufferAttribute,Uint16BufferAttribute,FileLoader,MeshLambertMaterial,Mesh,Clock,Texture,NearestFilter,ImageLoader} from "three";

export class MinecraftModel {
	constructor() {
		this.textures = {};
		this.elements = [];
	}
}
function isMinecraftModel(model) {
	return (
		model &&
		model.textures &&
		Object.entries(model.textures).every(
			([name, texture]) =>
				typeof name === 'string' && typeof texture === 'string',
		) &&
		Array.isArray(model.elements) &&
		model.elements.every(isMinecraftModelElement)
	);
}
class MinecraftModelElement {
	constructor() {
		this.from = null;
		this.to = null;
		this.rotation = null;
		this.faces = {}
	}
}
function isMinecraftModelElement(element) {
	let faceCount;

	return (
		element &&
		isArrayVector3(element.from) &&
		isArrayVector3(element.to) &&
		(element.rotation === undefined ||
			isMinecraftModelElementRotation(element.rotation)) &&
		element.faces &&
		(faceCount = Object.keys(element.faces).length) >= 1 &&
		faceCount <= 6 &&
		[
			element.faces.down,
			element.faces.up,
			element.faces.north,
			element.faces.south,
			element.faces.west,
			element.faces.east,
		].every((face) => face === undefined || isMinecraftModelFace(face))
	);
}
function isMinecraftModelElementRotation(rotation) {
	return (
		rotation &&
		isArrayVector3(rotation.origin) &&
		ELEMENT_ROTATION_ANGLES.includes(rotation.angle) &&
        ELEMENT_ROTATION_AXIS_VALUES.includes(rotation.axis) &&
		(rotation.rescale === undefined || typeof rotation.rescale === 'boolean')
	);
}
class MinecraftModelElementRotation {
	constructor() {
		this.origin = null;
		this.angle = null;
		this.axis = null;
		this.rescale = false;
	}
}
class MinecraftModelFace {
	constructor() {
		this.texture = "";
		this.uv = null;
		this.rotation = null;
	}
}
function isMinecraftModelFace(face) {
	return (
		face &&
		typeof face.texture === 'string' &&
		face.texture.length >= 2 &&
		face.texture[0] === '#' &&
		(face.uv === undefined || isArrayVector4(face.uv)) &&
		(face.rotation === undefined || TEXTURE_ROTATION_ANGLES.includes(face.rotation))
	);
}
const MinecraftModelFaceName = {
    WEST: 'west',
	EAST: 'east',
	DOWN: 'down',
	UP: 'up',
	NORTH: 'north',
	SOUTH: 'south',
}
const MINECRAFT_MODEL_FACE_NAME_VALUES = [
    MinecraftModelFaceName.WEST,
    MinecraftModelFaceName.EAST,
    MinecraftModelFaceName.DOWN,
    MinecraftModelFaceName.UP,
    MinecraftModelFaceName.NORTH,
    MinecraftModelFaceName.SOUTH,
];
const vertexMaps = {
	west: [0, 1, 2, 3],
	east: [4, 5, 6, 7],
	down: [0, 3, 4, 7],
	up: [2, 1, 6, 5],
	north: [7, 6, 1, 0],
	south: [3, 2, 5, 4],
};
function applyVertexMapRotation(rotation, [a, b, c, d]) {
	return (rotation === 0
		? [a, b, c, d]
		: rotation === 90
		? [b, c, d, a]
		: rotation === 180
		? [c, d, a, b]
		: [d, a, b, c]);
}
function buildMatrix(angle, scale, axis) {
	const a = Math.cos(angle) * scale;
	const b = Math.sin(angle) * scale;
	const matrix = new Matrix3();

	if (axis === 'x') {
		matrix.set(1, 0, 0, 0, a, -b, 0, b, a);
	} else if (axis === 'y') {
		matrix.set(a, 0, b, 0, 1, 0, -b, 0, a);
	} else {
		matrix.set(a, -b, 0, b, a, 0, 0, 0, 1);
	}

	return matrix;
}
function rotateCubeCorners(corners, rotation) {
	const origin = new Vector3().fromArray(rotation.origin).subScalar(8);

	const angle = (rotation.angle / 180) * Math.PI;
	const scale = rotation.rescale
		? Math.SQRT2 / Math.sqrt(Math.cos(angle || Math.PI / 4) ** 2 * 2)
		: 1;
	const matrix = buildMatrix(angle, scale, rotation.axis);

	return corners.map(vertex =>
		new Vector3()
			.fromArray(vertex)
			.sub(origin)
			.applyMatrix3(matrix)
			.add(origin)
			.toArray(),
	);
}
function getCornerVertices(from, to) {
	const [x1, y1, z1, x2, y2, z2] = from
		.concat(to)
		.map((coordinate) => coordinate - 8);

	return [
		[x1, y1, z1],
		[x1, y2, z1],
		[x1, y2, z2],
		[x1, y1, z2],
		[x2, y1, z2],
		[x2, y2, z2],
		[x2, y2, z1],
		[x2, y1, z1],
	];
}

function generateDefaultUvs(faceName, [x1, y1, z1], [x2, y2, z2]) {
	return (faceName === 'west'
		? [z1, 16 - y2, z2, 16 - y1]
		: faceName === 'east'
		? [16 - z2, 16 - y2, 16 - z1, 16 - y1]
		: faceName === 'down'
		? [x1, 16 - z2, x2, 16 - z1]
		: faceName === 'up'
		? [x1, z1, x2, z2]
		: faceName === 'north'
		? [16 - x2, 16 - y2, 16 - x1, 16 - y1]
		: [x1, 16 - y2, x2, 16 - y1]);
}
function computeNormalizedUvs(uvs) {
	return uvs.map(
		(coordinate, i) =>
			(i % 2 ? 16 - coordinate : coordinate) / 16,
	);
}
export class MinecraftModelGeometry extends BufferGeometry {
	constructor(model) {
		super();
		const {
			vertices,
			uvs,
			indices,
			groups,
		} = MinecraftModelGeometry.computeAttributes(model);

		this.setAttribute('position', new Float32BufferAttribute(vertices, 3));
		this.setAttribute('uv', new Float32BufferAttribute(uvs, 2));
		this.setIndex(new Uint16BufferAttribute(indices, 1));

		for (const { start, count, materialIndex } of groups)
			this.addGroup(start, count, materialIndex);
		
		this.computeVertexNormals();
		this.computeBoundsTree();
	}

	static computeAttributes(model) {
		const builder = new GroupedAttributesBuilder(model.textures);

		for (const element of model.elements) {
			const { from, to, rotation } = element;
			const cornerVertices = getCornerVertices(from, to);
			const rotatedVertices = rotation
				? rotateCubeCorners(cornerVertices, rotation)
				: cornerVertices;

			for (const name in element.faces) {
				const faceName = name;
				const face = element.faces[faceName];

				if (face === undefined) continue;
				
				var texture = face.texture.substring(1);
				while(model.textures[texture] && model.textures[texture][0] === '#') texture = model.textures[texture].substring(1);

				const { vertices, uvs, indices } = builder.getContext(
					"#" + texture,
				);

				const i = vertices.length / 3;
				indices.push(i, i + 2, i + 1);
				indices.push(i, i + 3, i + 2);

				for (const index of applyVertexMapRotation(
					face.rotation || 0,
					vertexMaps[faceName],
				))
					vertices.push(...rotatedVertices[index]);

				const faceUvs =
					face.uv || generateDefaultUvs(faceName, from, to);
				const [u1, v1, u2, v2] = computeNormalizedUvs(faceUvs);

				uvs.push(u1, v2);
				uvs.push(u1, v1);
				uvs.push(u2, v1);
				uvs.push(u2, v2);
			}
		}

		return builder.getAttributes();
	}
}
export class AbstractLoader {
	constructor(manager) {
		this.path = '';
	}

	load(url, onLoad, onProgress, onError) {
		throw new Error("Not implemented");
	}

	setPath(value) {
		this.path = value;
		return this;
	}
}
export class MinecraftModelLoader extends AbstractLoader {
	load(url, onLoad, onProgress, onError) {
		const loader = new FileLoader(this.manager);
		loader.setPath(this.path);
		loader.setResponseType('json');

		const handleLoad = async (model) => {
			try {
				var bakedModel = model;
				while(bakedModel.parent) {
					var parentmodel = await this.resolveParentModel(bakedModel, bakedModel.parent) ?? {};
					bakedModel = mergician({})(parentmodel, bakedModel);
					bakedModel.parent = parentmodel.parent;
				}
				
				const mesh = new MinecraftModelMesh(bakedModel);

				if (onLoad) onLoad(mesh);
			} catch (err) {
				if (onError) onError(err);
			}
		};

		loader.load(url, handleLoad, onProgress, onError);
	}
}
export class MinecraftModelMaterial extends MeshLambertMaterial {
	constructor(map) {
		super({
			map: map ?? new MinecraftTexture(),
			transparent: true,
			alphaTest: 0.01
		});
	}
}
export class MinecraftModelMesh extends Mesh {

	constructor(model) {
		if (typeof model === 'string') model = JSON.parse(model);

		if (!isMinecraftModel(model)) throw new Error('Invalid model');

		const geometry = new MinecraftModelGeometry(model);

		const sortedTextures = [
			...new Set(Object.values(model.textures)),
		].sort();
		const mapping = {};
		const materials = sortedTextures.map(
			path => (mapping[path] = new MinecraftModelMaterial()),
		);

		super(geometry, [new MinecraftModelMaterial(), ...materials]);

		this.materialMapping = mapping;
	}

	resolveTextures(resolver) {
		for (const path in this.materialMapping) {
			this.materialMapping[path].map = resolver(path);
		}
	}
}
const CHECKERBOARD_IMAGE = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAIAAACQkWg2AAAACXBIWXMAAC4jAAAuIwF4pT92AAAAB3RJTUUH4goSFSEEtucn/QAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAAAkSURBVCjPY2TAAX4w/MAqzsRAIhjVQAxgxBXeHAwco6FEPw0A+iAED8NWwMQAAAAASUVORK5CYII=';
const MCTextures = [];
const MCTextureClock = new Clock();
export class MinecraftTexture extends Texture {
	constructor(image) {
		super();
		this._image = image;
		this.magFilter = NearestFilter;
		this.frames = [{tile: 0, time:1}];
		this.currentDisplayTime = 0;
		this.currentTile = 0;
		this.tileAmount = 1;
		this.tileHeight = 16;
		this.ratio = 1;
		this.generateMipmaps = false;
		this.update = function( milliSec )
		{
			var ret = false;
			this.currentDisplayTime += milliSec;
			while(this.currentDisplayTime > this.frames[this.currentTile].time)
			{
				this.currentDisplayTime -= this.frames[this.currentTile++].time;
				if (this.currentTile == this.frames.length)
					this.currentTile = 0;
				ret = true;
			}
			this.offset.y = (this.tileAmount - this.frames[this.currentTile].tile - 1) * this.repeat.y;
			return ret;
		};
	}
	
	static update() {
		var delta = MCTextureClock.getDelta();
		var ret = false;
		MCTextures.forEach(e => ret = e.update(20 * delta) || ret);
		return ret;
	}
	
	get image() {
		return this._image;
	}

	set image(value) {
		if(value) {
			this._image = value;
			if(value.width != value.height) {
				const loader = new FileLoader();
				loader.setResponseType('json');
				var self = this;
				loader.load(value.src + ".mcmeta", (data) => {
					self.ratio = (data.animation.width ?? 1)/(data.animation.height ?? 1);
					self.tileHeight = Math.ceil(value.width * self.ratio);
					self.tileAmount = Math.ceil(value.height / self.tileHeight);
					self.repeat.y = 1 / value.height * self.tileHeight;
					if(data.animation.frames) {
						self.frames = [];
						data.animation.frames.forEach(t => {
							if(typeof t === 'object')
								self.frames.push({tile: t.index, time:t.time ?? data.animation.frametime ?? 1});
							else
								self.frames.push({tile: t, time:data.animation.frametime ?? 1});
						});
					} else
						for(var i = 0;i < self.tileAmount;i++)
							self.frames.push({tile: i, time:data.animation.frametime ?? 1});
					if(!MCTextures.includes(self)) MCTextures.push(self);
				});
			} else
				this.tileHeight = value.height;
		} else this._image = new ImageLoader().load(CHECKERBOARD_IMAGE);
		this.needsUpdate = true;
	}
}
export class MinecraftTextureLoader extends AbstractLoader {
	constructor() {
		super();
		this.crossOrigin = 'anonymous';
	}

	load(url, onLoad, onProgress, onError) {
		const texture = new MinecraftTexture();

		const loader = new ImageLoader(this.manager);
		loader.setCrossOrigin(this.crossOrigin);
		loader.setPath(this.path);

		const handleLoad = (image) => {
			texture.image = image;
			if (onLoad) onLoad(texture);
		};
		
		const handleError = (error) => {
			texture.image = null;
			if (onError) onError(error);
		};

		loader.load(url, handleLoad, onProgress, handleError);

		return texture;
	}

	setCrossOrigin(value) {
		this.crossOrigin = value;
		return this;
	}
}
function isArrayVector3(arrayVector) {
	return (
		Array.isArray(arrayVector) &&
		arrayVector.length === 3 &&
		arrayVector.every(coordinate => typeof coordinate === 'number')
	);
}
function isArrayVector4(arrayVector) {
	return (
		Array.isArray(arrayVector) &&
		arrayVector.length === 4 &&
		arrayVector.every(coordinate => typeof coordinate === 'number')
	);
}
const ELEMENT_ROTATION_ANGLES = [-45, -22.5, 0, 22.5, 45];
const ElementRotationAxis = {
    X: 'x',
    Y: 'y',
    Z: 'z',
};
const ELEMENT_ROTATION_AXIS_VALUES = [
    ElementRotationAxis.X, 
    ElementRotationAxis.Y, 
    ElementRotationAxis.Z
];
class GroupAttributes {
	constructor() {
		this.vertices = [];
		this.uvs = [];
		this.indices = [];
	}
}
class GroupedAttributesBuilder {
	constructor(textures) {
		this.groups = {};
		this.groupMapping = {};
		this.missingGroup = new GroupAttributes();
		
		for (const texturePath of new Set(Object.values(textures))) {
			this.groups[texturePath] = { vertices: [], uvs: [], indices: [] };
		}

		for (const variable in textures) {
			this.groupMapping['#' + variable] = this.groups[textures[variable]];
		}
	}

	getContext(textureVariable) {
		return this.groupMapping[textureVariable] || this.missingGroup;
	}

	getAttributes() {
		let { vertices, uvs, indices } = this.missingGroup;
		let indexCount = indices.length;

		const groups = [{ start: 0, count: indexCount, materialIndex: 0 }];

		groups.push(
			...Object.keys(this.groups)
				.sort()
				.map((path, i) => {
					const group = this.groups[path];

					const start = indexCount;
					const count = group.indices.length;
					const offset = vertices.length / 3;

					vertices = vertices.concat(group.vertices);
					uvs = uvs.concat(group.uvs);
					indices = indices.concat(
						group.indices.map(index => index + offset),
					);

					indexCount += count;

					return { start, count, materialIndex: i + 1 };
				}),
		);

		return { vertices, uvs, indices, groups };
	}
}
const TEXTURE_ROTATION_ANGLES = [0, 90, 180, 270];